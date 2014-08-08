using System;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.Infrastructure
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IRepository<User> _userRepository;
        private IRepository<UserSocialNetwork> _userSNRepository;
        private IRepository<SocialNetwork> _snRepository;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool SaveOrUpdateUserData(User user, string socialId, string socialNetwork, string accessToken, string tokenExpiresIn)
        {
            using (_unitOfWork)
            {
                try
                {
                    // Get repositories
                    _userRepository = _unitOfWork.GetRepository<User>();
                    _userSNRepository = _unitOfWork.GetRepository<UserSocialNetwork>();
                    _snRepository = _unitOfWork.GetRepository<SocialNetwork>();

                    // Check wheter current user exists in database
                    var getUserFromDB = _userSNRepository.GetOne(s => s.ThirdPartId == socialId);
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
                        var currentSN = _snRepository.GetOne(s => s.Name == socialNetwork);

                        _userRepository.AddItem(user);
                        _userSNRepository.AddItem(new UserSocialNetwork
                        {
                            Identifier = new UserSNIdentifier {User = user, SocialNetwork = currentSN},
                            ThirdPartId = socialId,
                            AccessToken = accessToken,
                            TokenExpires = tokenExpiresIn
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

        public bool CheckUserInDB(string socialId)
        {
            if (!((UnitOfWork)_unitOfWork).CurrentSession.IsOpen)
            {
                ((UnitOfWork) _unitOfWork).ReopenSession();
            }
            using (_unitOfWork)
            {
                _userSNRepository = _unitOfWork.GetRepository<UserSocialNetwork>();

                var userSN = _userSNRepository.GetOne(s => s.ThirdPartId == socialId);
                if ((userSN != null) && (userSN.Identifier.User != null))
                        return true;
            }
            return false;
        }
    }

    public interface IAccountService
    {
        bool SaveOrUpdateUserData(User user, string socialId, string socialNetwork, string accessToken, string tokenExpiresIn);
        bool CheckUserInDB(string socialId);
    }
}