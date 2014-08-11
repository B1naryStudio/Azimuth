using System;
using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.DataProviders.Interfaces;
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

        public bool SaveOrUpdateUserData(User user, UserCredential userCredential, bool isAuthenticated)
        {
            using (_unitOfWork)
            {
                try
                {
                    var userSn = _userSNRepository.GetByThirdPartyId(userCredential.SocialNetworkId);
                    if (userSn != null)
                    {
                        if (isAuthenticated)
                        {
                            var loggedUser = _userRepository.GetUserByEmail(userCredential.Email);
                            userSn.Identifier.User.Id = loggedUser.Id;
                        }
                        // If user exists in database check his data fields for updating
                        if (user.ToString() != userSn.Identifier.User.ToString())
                        {
                            userSn.Identifier.User.Name = new Name { FirstName = user.Name.FirstName, LastName = user.Name.LastName };
                            userSn.Identifier.User.ScreenName = user.ScreenName;
                            userSn.Identifier.User.Gender = user.Gender;
                            userSn.Identifier.User.Email = user.Email;
                            userSn.Identifier.User.Birthday = user.Birthday;
                            userSn.Identifier.User.Location = new Location { Country = user.Location.Country, City = user.Location.City };
                            userSn.Identifier.User.Timezone = user.Timezone;
                            userSn.Identifier.User.Photo = user.Photo;
                        }
                    }
                    else
                    {
                        var currentSN = _snRepository.GetByName(userCredential.SocialNetworkName);

                        _userRepository.AddItem(user);
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
    }
}