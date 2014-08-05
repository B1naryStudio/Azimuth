using System;
using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Ninject;

namespace Azimuth.Infrastructure
{
    public class DatabaseProvider
    {
        private readonly IKernel _kernel;
        private IRepository<User> _userRepository;
        private IRepository<UserSocialNetwork> _userSNRepository;
        private IRepository<SocialNetwork> _snRepository; 
        public DatabaseProvider()
        {
            _kernel = new StandardKernel(new DataAccessModule());
        }

        public bool SaveOrUpdateUserData(User user, string socialId, string socialNetwork, string accessToken, string tokenExpiresIn)
        {
            using (var unitOfWork = _kernel.Get<IUnitOfWork>())
            {
                try
                {
                    // Get repositories
                _userRepository = unitOfWork.GetRepository<User>();
                _userSNRepository = unitOfWork.GetRepository<UserSocialNetwork>();
                _snRepository = unitOfWork.GetRepository<SocialNetwork>();

                // Check wheter current user exists in database
                var getUserFromDB =
                    _userSNRepository.Get(s => s.ThirdPartId == socialId).ToList();
                if (getUserFromDB.Count != 0)
                {
                    // If user exists in database check his data fields for updating
                    if (user.ToString() != getUserFromDB[0].Identifier.User.ToString())
                    {
                        getUserFromDB[0].Identifier.User.Name = new Name() { FirstName = user.Name.FirstName, LastName = user.Name.LastName };
                        getUserFromDB[0].Identifier.User.ScreenName = user.ScreenName;
                        getUserFromDB[0].Identifier.User.Gender = user.Gender;
                        getUserFromDB[0].Identifier.User.Email = user.Email;
                        getUserFromDB[0].Identifier.User.Birthday = user.Birthday;
                        getUserFromDB[0].Identifier.User.Location = new Location() { Country = user.Location.Country, City = user.Location.City };
                        getUserFromDB[0].Identifier.User.Timezone = user.Timezone;
                    }
                }
                else
                {
                    var currentSN = _snRepository.Get(s => s.Name == socialNetwork).ToList();

                    _userRepository.AddItem(user);
                    _userSNRepository.AddItem(new UserSocialNetwork()
                    {
                        Identifier = new UserSNIdentifier {User = user, SocialNetwork = currentSN[0]},
                        ThirdPartId = socialId,
                        AccessToken = accessToken,
                        TokenExpires = tokenExpiresIn
                    });
                }

                unitOfWork.Commit();
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
    }
}