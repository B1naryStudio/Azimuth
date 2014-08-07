using System;
using System.Linq;
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
    public class FacebookAccountProviderTests
    {
        // Classes for write JsonSerialization
        public class Location
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Data
        {
            public bool is_silhouette { get; set; }
            public string url { get; set; }
        }

        public class Picture
        {
            public Data data { get; set; }
        }

        public class RootObject
        {
            public string id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string name { get; set; }
            public string gender { get; set; }
            public string birthday { get; set; }
            public string email { get; set; }
            public int timezone { get; set; }
            public Location location { get; set; }
            public Picture picture { get; set; }
        }

        
        // Fields for tests
        private string _userToJson;

        private string _accessToken;
        private string _userId;

        //private ConstructorArgument _accessTokenParam;
        //private ConstructorArgument _userIdParam;

        public string UserInfoUrl { get; set; }

        private IWebClient _webRequest;

        private RootObject _fbUserData;

        [SetUp]
        public void Setup()
        {
            // Parametres for calling GetUSerInfoAsync
            _accessToken = "some access token";
            _userId = "some user id";

            //_accessTokenParam = new ConstructorArgument("accessToken", _accessToken);
            //_userIdParam = new ConstructorArgument("userId", _userId);

            // Query to facebook api
            UserInfoUrl = String.Format(@"https://graph.facebook.com/v2.0/me?access_token={0}&fields=id,first_name,last_name,name,gender,email,birthday,timezone,location,picture.type(large)",
                    _accessToken);

            // Object that we will make Json
            _fbUserData = new RootObject
            {
                id = "12345",
                first_name = "Beseda",
                last_name = "Dmitrij",
                name = "Beseda Dmitrij",
                gender = "male",
                email = "besedadg@gmail.com",
                birthday = "12/1/1992",
                timezone = 2,
                location = new Location { id = "1", name = "Donetsk, Ukraine" },
                picture = new Picture
                {
                    data = new Data
                    {
                        url = "photo.jpg"
                    }
                }
            };

            _userToJson = JsonConvert.SerializeObject(_fbUserData);

            _webRequest = Substitute.For<IWebClient>();
            _webRequest.GetWebData(UserInfoUrl).Returns(Task.FromResult(_userToJson));
        }

        [Test]
        public async void Get_Facebook_User_Data()
        {
            // Arrange
            User expectedUser = new User
            {
                Name = new Name
                {
                    FirstName = _fbUserData.first_name,
                    LastName = _fbUserData.last_name
                },
                ScreenName = _fbUserData.name,
                Gender = _fbUserData.gender,
                Email = _fbUserData.email,
                Birthday = _fbUserData.birthday,
                Timezone = _fbUserData.timezone,
                Location = new DataAccess.Entities.Location
                {
                    City = _fbUserData.location.name.Split(',').First(),
                    Country = _fbUserData.location.name.Split(' ').Last()
                },
                Photo = _fbUserData.picture.data.url
            };
            // Act
            var provider = new FacebookAccountProvider(_webRequest, _userId, _accessToken);
            var user = await provider.GetUserInfoAsync("besedadg@gmail.com");
            // Assert
            user.ToString().Should().Be(expectedUser.ToString(), "");
        }
    }
}
