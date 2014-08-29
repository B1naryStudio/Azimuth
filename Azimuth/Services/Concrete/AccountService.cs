using System;
using System.IdentityModel.Claims;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.DataProviders.Concrete;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Claim = System.Security.Claims.Claim;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace Azimuth.Services.Concrete
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserRepository _userRepository;
        private readonly UserSocialNetworkRepository _userSNRepository;
        private readonly SocialNetworkRepository _snRepository;
        private readonly PlaylistRepository _playlistRepository;

        public async Task<bool> LoginCallback(bool autoLogin)
        {
            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);

            if (result == null || result.Identity == null)
            {
                return false;
            }

            var principal = ClaimsAuthenticationManager.Authenticate(String.Empty, new ClaimsPrincipal(result.Identity));
            var identity = principal.Identity as AzimuthIdentity;
            var loggedIdentity = AzimuthIdentity.Current;

            if (identity == null)
            {
                return true;
            }

            var provider = AccountProviderFactory.GetAccountProvider(identity.UserCredential);

            var userInfo = await provider.GetUserInfoAsync(identity.UserCredential.Email);
            var storeResult = SaveOrUpdateUserData(userInfo, identity.UserCredential, loggedIdentity);

            if (storeResult && autoLogin)
            {
                SignIn(identity, userInfo);
            }
            return true;
        }

        public void SignOut()
        {
            AuthenticationManager.SignOut();
        }

        public ClaimsAuthenticationManager ClaimsAuthenticationManager
        {
            get
            {
                return FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager;
            }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Get repositories
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _userSNRepository = _unitOfWork.GetRepository<UserSocialNetwork>() as UserSocialNetworkRepository;
            _snRepository = _unitOfWork.GetRepository<SocialNetwork>() as SocialNetworkRepository;
            _playlistRepository = _unitOfWork.GetRepository<Playlist>() as PlaylistRepository;
        }

        public bool SaveOrUpdateUserData(User user, UserCredential userCredential, AzimuthIdentity loggedIdentity)
        {
            using (_unitOfWork)
            {
                try
                {
                    User loggedUser = null;
                    if (loggedIdentity != null)
                    {
                        loggedUser = _userRepository.GetOne(x => x.Email == loggedIdentity.UserCredential.Email);
                    }
                    var userSn = _userSNRepository.GetByThirdPartyId(userCredential.SocialNetworkId);
                    if (userSn != null)
                    {
                        if (loggedUser != null)
                        {
                            user.Id = loggedUser.Id;
                            // If we login again with the same social network, skip updating
                            if (loggedUser.Id != userSn.User.Id)
                            {
                                //var userPlaylists = _playlistRepository.GetByCreatorId(userSn.User.Id);
                                var userPlaylists = _playlistRepository.Get(s => s.Creator.Id == userSn.User.Id);

                                foreach (var userPlaylist in userPlaylists)
                                {
                                    userPlaylist.Creator = loggedUser;
                                }

                                var userToDelete = userSn.User;
                                userSn.User = loggedUser;
                                _userSNRepository.ChangeUserId(userSn);
                                _userRepository.DeleteItem(userToDelete);    
                            }
                            else
                            {
                                userSn.AccessToken = userCredential.AccessToken;
                            }
                        }
                        else
                        {
                            // If user exists in database check his data fields for updating
                            if (user.ToString() != userSn.User.ToString())
                            {
                                userSn.User.Name = new Name
                                {
                                    FirstName = user.Name.FirstName,
                                    LastName = user.Name.LastName
                                };
                                userSn.User.ScreenName = user.ScreenName;
                                userSn.User.Gender = user.Gender;
                                userSn.User.Email = user.Email;
                                userSn.User.Birthday = user.Birthday;
                                userSn.User.Location = new Location
                                {
                                    Country = user.Location.Country,
                                    City = user.Location.City
                                };
                                userSn.User.Timezone = user.Timezone;
                                userSn.User.Photo = userSn.Photo = user.Photo ?? String.Empty;
                                userSn.UserName = (user.Name.FirstName ??
                                    String.Empty) + ((user.Name.LastName != null) ? (" " + user.Name.LastName) : String.Empty);
                            }
                        }
                    }
                    else
                    {
                        var currentSN = _snRepository.GetByName(userCredential.SocialNetworkName);

                        if (loggedIdentity == null)
                        {
                            _userRepository.AddItem(user);
                        }
                        _userSNRepository.AddItem(new UserSocialNetwork
                        {
                            User = loggedUser ?? user, 
                            SocialNetwork = currentSN,
                            ThirdPartId = userCredential.SocialNetworkId,
                            AccessToken = userCredential.AccessToken,
                            TokenExpires = userCredential.AccessTokenExpiresIn,
                            Photo = user.Photo,
                            UserName = (user.Name.FirstName ??
                                    String.Empty) + ((user.Name.LastName != null) ? (" " + user.Name.LastName) : String.Empty)
                        });
                    }

                    _unitOfWork.Commit();
                }
                catch (Exception)
                {
                    _unitOfWork.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool DisconnectUserAccount(string provider)
        {
            using (_unitOfWork)
            {
                try
                {
                    var user = _userRepository.GetOne(x => x.Email == AzimuthIdentity.Current.UserCredential.Email);
                    var socialNetwork = _snRepository.GetOne(x => x.Name == provider);
                    if (user == null || socialNetwork == null)
                    {
                        throw new ApplicationException(
                            string.Format("Can't find user or social network (email: {0}, SN name: {1}",
                                AzimuthIdentity.Current.UserCredential.Email, provider));
                    }
                    _userSNRepository.Remove(user.Id, socialNetwork.Id);

                    _unitOfWork.Commit();
                }
                catch (Exception)
                {
                    _unitOfWork.Rollback();
                    return false;
                }
            }
            return true;
        }

        public void SignIn(AzimuthIdentity identity, User userInfo)
        {
            identity.AddClaim(new Claim(ClaimTypes.Name, userInfo.Name.FirstName + " " + userInfo.Name.LastName));
            identity.AddClaim(new Claim(AzimuthClaims.PHOTO_BIG, userInfo.Photo));
            identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                identity.UserCredential.SocialNetworkName, Rights.PossessProperty));

            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);
        }
    }
}