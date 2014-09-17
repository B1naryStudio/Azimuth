using System;
using System.IdentityModel.Claims;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.DataProviders.Concrete;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NHibernate.Action;
using WebGrease.Css.Extensions;
using Claim = System.Security.Claims.Claim;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace Azimuth.Services.Concrete
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

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

        public AccountService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public bool SaveOrUpdateUserData(User user, UserCredential userCredential, AzimuthIdentity loggedIdentity)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var _userSNRepository = unitOfWork.GetTypedRepository<IUserSocialNetworkRepository>();
                var _snRepository = unitOfWork.GetTypedRepository<ISocialNetworkRepository>();
                var _playlistRepository = unitOfWork.GetRepository<Playlist>();
                try
                {
                    User loggedUser = null;
                    if (loggedIdentity != null)
                    {
                        loggedUser = unitOfWork.UserRepository.GetOne(x => x.Email == loggedIdentity.UserCredential.Email);
                    }
                    var userSn = _userSNRepository.GetByThirdPartyId(userCredential.SocialNetworkId);
                    if (userSn != null)
                    {
                        if (loggedUser != null)
                        {
                            user.Id = loggedUser.Id;
                            // If we login again with the same social network, skip updating (merging accounts)
                            if (loggedUser.Id != userSn.User.Id)
                            {
                                var playlistLikerRepository =
                                        unitOfWork.GetTypedRepository<PlaylistLikerRepository>();
                                var notificationRepository =
                                    unitOfWork.GetTypedRepository<INotificationRepository>();
                                var userRepository = unitOfWork.GetTypedRepository<IUserRepository>();

                                var allAccounts =
                                    _userSNRepository.Get(account => account.User.Id == userSn.User.Id).ToList();
                                var userToDelete = userSn.User;
                                foreach (var userSocialNetwork in allAccounts)
                                {
                                    // Getting unlogged user's account playlists
                                    var userPlaylists = _playlistRepository.Get(s => s.Creator.Id == userSocialNetwork.User.Id).ToList();
                                    // Taking playlists which he liked or favourited


                                    var notUserPlaylists = playlistLikerRepository.Get(s => s.Liker.Id == userSocialNetwork.User.Id).ToList();
                                    if (notUserPlaylists.Any())
                                        userPlaylists.AddRange(notUserPlaylists.Select(s => s.Playlist));

                                    // Find if user liked/favourited his another account's playlists 
                                    var recordsTodelete =
                                        userPlaylists.SelectMany(s => s.PlaylistLikes).Where(
                                            item => item.Playlist.Creator.Id == loggedUser.Id).ToList();
                                    // Getting user's unauthorized account activity
                                    var notifications =
                                        notificationRepository.Get(item => item.User.Id == userSocialNetwork.User.Id).ToList();

                                    foreach (var playlistLike in recordsTodelete)
                                    {
                                        // Notifications like/unlike and favourite/unfavourited of his another account playlists should be deleted
                                        var notificationsToDelete = notifications.Where(
                                            s =>
                                                s.RecentlyPlaylist == playlistLike.Playlist &&
                                                s.NotificationType == Notifications.FavoritedPlaylist ||
                                                s.NotificationType == Notifications.LikedPlaylist ||
                                                s.NotificationType == Notifications.UnfavoritedPlaylist ||
                                                s.NotificationType == Notifications.UnlikedPlaylist).ToList();

                                        // Get all records of unauthorizedd user's account from likes table
                                        var likeRecord = playlistLikerRepository.GetOne(item => item.Id == playlistLike.Id);
                                        // Kill connection between notifications which would be deleted with playlists
                                        foreach (var notification in notificationsToDelete)
                                        {
                                            likeRecord.Playlist.Notifications.Remove(notification);
                                        }

                                        // Delete notifications
                                        notifications.Where(
                                            s =>
                                                s.RecentlyPlaylist == playlistLike.Playlist &&
                                                (s.NotificationType == Notifications.FavoritedPlaylist ||
                                                s.NotificationType == Notifications.LikedPlaylist ||
                                                s.NotificationType == Notifications.UnfavoritedPlaylist ||
                                                s.NotificationType == Notifications.UnlikedPlaylist)).ForEach(item => notificationRepository.DeleteItem(item));

                                        // Kill connection playlist -> like record of playlist (like/unlike && favourite/unfavouriteown playlists of another account)
                                        likeRecord.Playlist.PlaylistLikes.Remove(playlistLike);
                                        // Kill connection user -> like record of user (like/unlike && favourite/unfavouriteowne own playlists of another account)
                                        likeRecord.Liker.PlaylistFollowing.Remove(playlistLike);
                                        // Delete like/unlike && favourite/unfavouriteowne  playlists of another users's account 
                                        playlistLikerRepository.DeleteItem(playlistLike);
                                    }
                                    // Normal user's activity should change user link
                                    userPlaylists.SelectMany(list => list.PlaylistLikes).ForEach(item => item.Liker = loggedUser);
                                    // Added normal user's playlists should also change user link
                                    userPlaylists.ForEach(item => item.Creator = loggedUser);
                                    // Other notifications have to change their user
                                    notifications.Where(
                                        s =>
                                            s.User.Id == userSocialNetwork.User.Id).ForEach(item => item.User = loggedUser);

                                    userSocialNetwork.User = loggedUser;
                                    _userSNRepository.ChangeUserId(userSocialNetwork);

                                }
                                userRepository.DeleteItem(userToDelete);
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
                            user.Id = userSn.User.Id;
                        }
                    }
                    else
                    {
                        var currentSN = _snRepository.GetByName(userCredential.SocialNetworkName);

                        if (loggedIdentity == null)
                        {
                            unitOfWork.UserRepository.AddItem(user);
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

                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool DisconnectUserAccount(string provider)
        {
            using (var _unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var _userRepository = _unitOfWork.GetTypedRepository<IUserRepository>();
                var _userSNRepository = _unitOfWork.GetTypedRepository<IUserSocialNetworkRepository>();
                var _snRepository = _unitOfWork.GetTypedRepository<ISocialNetworkRepository>();
                var _playlistRepository = _unitOfWork.GetRepository<Playlist>();
                try
                {
                    var user = _userRepository.GetOne(x => x.Id == AzimuthIdentity.Current.UserCredential.Id);
                    var socialNetwork = _snRepository.GetOne(x => x.Name == provider);
                    if (user == null || socialNetwork == null)
                    {
                        throw new ApplicationException(
                            string.Format("Can't find user or social network (email: {0}, SN name: {1}",
                                AzimuthIdentity.Current.UserCredential.Email, provider));
                    }
                    var entity =
                        _userSNRepository.GetOne(usn => usn.User.Id == AzimuthIdentity.Current.UserCredential.Id && usn.SocialNetwork.Id == socialNetwork.Id);

                    user.SocialNetworks.Remove(entity);
                    _userSNRepository.DeleteItem(entity);

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
            identity.AddClaim(new Claim(AzimuthClaims.ID, userInfo.Id.ToString()));
            identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                identity.UserCredential.SocialNetworkName, Rights.PossessProperty));

            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);
        }
    }
}