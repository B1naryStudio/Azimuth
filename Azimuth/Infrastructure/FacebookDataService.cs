using System;
using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Shared.Dto;
using Facebook;
using Newtonsoft.Json;
using Ninject;

namespace Azimuth.Infrastructure
{
    public class FacebookDataService
    {
        private readonly string _accessToken;
        private readonly string _tokenExpiresIn;
        private readonly IKernel _kernel;
        private IRepository<User> _userRepository;
        private IRepository<UserSocialNetwork> _userSNRepository;
        private IRepository<SocialNetwork> _snRepository; 

        public FacebookDataService(string accessToken, string tokenExpiresIn)
        {
            this._accessToken = accessToken;
            this._tokenExpiresIn = tokenExpiresIn;

            _kernel = new StandardKernel(new DataAccessModule());
        }

        private string MakeRequestString()
        {
            return "/me?fields=id,first_name,last_name,name,gender,email,birthday,timezone,location,picture.type(large)";
        }

        public User GetUserData()
        {
            // Make query to Facebook Api
            var snClient = new FacebookClient(_accessToken);
            var userDataJson = snClient.Get(MakeRequestString());
            // Parse Facebook response from Json
            var userData = JsonConvert.DeserializeObject<FacebookUserData>(userDataJson.ToString());
            User user = new User();
            using (var unitOfWork = _kernel.Get<IUnitOfWork>())
            {
                _userSNRepository= unitOfWork.GetRepository<UserSocialNetwork>();
                // Check whether current user exists in database
                var getUser =
                    _userSNRepository.Get(new Func<UserSocialNetwork, bool>(network => network.ThirdPartId == userData.id)).ToList();
                if (getUser.Count != 0)
                {
                    // If current user exist check his data to update
                    _userRepository = unitOfWork.GetRepository<User>();
                    var currentUser = _userRepository.Get(getUser[0].Identifier.User.Id);
                    if (currentUser.ToString() != userData.ToString())
                    {
                        currentUser.Name = new Name() {FirstName = userData.first_name, LastName = userData.last_name};
                        currentUser.ScreenName = userData.name;
                        currentUser.Gender = userData.gender;
                        currentUser.Birthday = userData.birthday;
                        currentUser.Email = userData.email;
                        currentUser.Location = new Azimuth.DataAccess.Entities.Location()
                        {
                            City = userData.Location.name.Split(',')[0],
                            Country = userData.Location.name.Split(' ')[1]
                        };
                        currentUser.Timezone = userData.timezone;
                        user = currentUser;
                    }
                }
                else
                {
                    // If current user doesn't exist in database, we have to create it
                    _userRepository = unitOfWork.GetRepository<User>();
                    _snRepository = unitOfWork.GetRepository<SocialNetwork>();

                    user = new User()
                    {
                        Name = new Name() {FirstName = userData.first_name, LastName = userData.last_name},
                        ScreenName = userData.name,
                        Gender = userData.gender,
                        Birthday = userData.birthday,
                        Email = userData.email,
                        Location =
                            new DataAccess.Entities.Location()
                            {
                                City = userData.Location.name.Split(',')[0],
                                Country = userData.Location.name.Split(' ')[1]
                            },
                        Timezone = userData.timezone
                    };

                    var snEnum = _snRepository.Get(new Func<SocialNetwork, bool>(network => network.Name == "Facebook"));
                        // Change "Facebook" to some generic.
                    var sn = snEnum.ToList();

                    _userRepository.AddItem(user);
                    _userSNRepository.AddItem(new UserSocialNetwork()
                    {
                        Identifier = new UserSNIdentifier() {User = user, SocialNetwork = sn[0]},
                        ThirdPartId = userData.id,
                        AccessToken = _accessToken,
                        TokenExpires = _tokenExpiresIn
                    });
                }

                unitOfWork.Commit();
            }

            return user;
        }
    }
}
