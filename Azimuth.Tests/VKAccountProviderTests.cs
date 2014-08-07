using System;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using Ninject.Parameters;
using NSubstitute;
using NUnit.Framework;

namespace Azimuth.Tests
{
    [TestFixture]
    class VKAccountProviderTests
    {
        // Classes for write JsonSerialization
        public class City
        {
            public int id { get; set; }
            public string title { get; set; }
        }

        public class Country
        {
            public int id { get; set; }
            public string title { get; set; }
        }

        public class RootObject
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public int sex { get; set; }
            public string screen_name { get; set; }
            public string bdate { get; set; }
            public City city { get; set; }
            public Country country { get; set; }
            public int timezone { get; set; }
            public string photo_max_orig { get; set; }
        }

        public class Response
        {
            public RootObject response { get; set; } 
        }

        public enum Sex
        {
            none = 0,
            female = 1,
            male = 2
        }

        // Fields for tests
        private string _userToJson;

        private string _accessToken;
        private string _userId;

        //private ConstructorArgument _accessTokenParam;
        //private ConstructorArgument _userIdParam;

        public string UserInfoUrl { get; set; }

        private IWebClient _webRequest;

        private RootObject _vkUserData;

        [SetUp]
        public void SetUp()
        {
            // Parametres for calling GetUserInfoAsync
            _userId = "some user id";
            _accessToken = "some access token";
            
            // Query to VK api
            UserInfoUrl = String.Format(
                @"https://api.vk.com/method/users.get?user_id={0}&fields=screen_name,bdate,sex,city,country,photo_max_orig,timezone&v=5.23&access_token={1}",
                _userId,
                _accessToken);

            // Object that we will make Json
            _vkUserData = new RootObject
            {
                first_name = "Beseda",
                last_name = "Dmitrij",
                screen_name = "Beseda Dmitrij",
                sex = (int)Sex.male,
                bdate = "12/1/1992",
                city = new City{id = 1, title = "Donetsk"},
                country = new Country {id = 1, title = "Ukraine"},
                timezone = 2,
                photo_max_orig = "photo.jpg"
            };


            // Make Json
            _userToJson = "{\"response\":[" + JsonConvert.SerializeObject(_vkUserData) + "]}";

            _webRequest = Substitute.For<IWebClient>();
            _webRequest.GetWebData(UserInfoUrl).Returns(Task.FromResult(_userToJson));
        }

        [Test]
        public async void Get_VK_User_Data()
        {
            string email = "besedadg@gmail.com";

            // Arrange
            User expectedUser = new User
            {
                Name = new Name
                {
                    FirstName = _vkUserData.first_name,
                    LastName = _vkUserData.last_name
                },
                ScreenName = _vkUserData.screen_name,
                Gender = Sex.male.ToString(),
                Email = email,
                Birthday = _vkUserData.bdate,
                Timezone = _vkUserData.timezone,
                Location = new DataAccess.Entities.Location
                {
                    City = _vkUserData.city.title,
                    Country = _vkUserData.country.title
                },
                Photo = _vkUserData.photo_max_orig
            };

            // Act
            var provider = new VKAccountProvider(_webRequest, _userId, _accessToken);
            var user = await provider.GetUserInfoAsync(email);
            // Assert
            user.ToString().Should().Be(expectedUser.ToString(), "");
        }
    }
}
