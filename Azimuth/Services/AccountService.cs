using System;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Infrastructure;

namespace Azimuth.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserRepository _userRepository;
        private readonly UserSocialNetworkRepository _userSNRepository;
        private readonly SocialNetworkRepository _snRepository;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Get repositories
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _userSNRepository = _unitOfWork.GetRepository<UserSocialNetwork>() as UserSocialNetworkRepository;
            _snRepository = _unitOfWork.GetRepository<SocialNetwork>() as SocialNetworkRepository;
        }

        public bool SaveOrUpdateUserData(User user, UserCredential userCredential, AzimuthIdentity loggedIdentity)
        {
            using (_unitOfWork)
            {
                try
                {
                    // Check wheter current user exists in database
                    var getUserFromDB = _userSNRepository.GetOne(s => s.ThirdPartId == userCredential.SocialNetworkId);
                    if (getUserFromDB != null)
                    {
                        // If user exists in database check his data fields for updating
                        if (user.ToString() != getUserFromDB.Identifier.User.ToString())
                        {
                            getUserFromDB.Identifier.User.Name = new Name { FirstName = user.Name.FirstName, LastName = user.Name.LastName };
                            getUserFromDB.Identifier.User.ScreenName = user.ScreenName;
                            getUserFromDB.Identifier.User.Gender = user.Gender;
                            getUserFromDB.Identifier.User.Email = user.Email;
                            getUserFromDB.Identifier.User.Birthday = user.Birthday;
                            getUserFromDB.Identifier.User.Location = new Location { Country = user.Location.Country, City = user.Location.City };
                            getUserFromDB.Identifier.User.Timezone = user.Timezone;
                            getUserFromDB.Identifier.User.Photo = user.Photo;
                        }
                    }
                    else
                    {
                        var currentSN = _snRepository.GetOne(s => s.Name == userCredential.SocialNetworkName);

                        if (loggedIdentity == null)
                        {
                            _userRepository.AddItem(user);
                        }
                        _userSNRepository.AddItem(new UserSocialNetwork
                        {
                            Identifier = new UserSNIdentifier {User = user, SocialNetwork = currentSN},
                            ThirdPartId = userCredential.SocialNetworkId,
                            AccessToken = userCredential.AccessToken,
                            TokenExpires = userCredential.AccessTokenExpiresIn
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
                }
                catch (Exception)
                {
                    _unitOfWork.Rollback();
                    return false;
                }
            }
            return true;
        }
    }
}