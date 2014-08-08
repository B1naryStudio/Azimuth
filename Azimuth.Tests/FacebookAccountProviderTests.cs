using System;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using Azimuth.Shared.Dto;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Azimuth.Tests
{
    [TestFixture]
    public class FacebookAccountProviderTests
    {
        // Fields for tests
        private string _userToJson;
        private const string _accessToken = "some access token";
        private const string _userId = "some user id";

        public string UserInfoUrl { get; set; }

        private IWebClient _webRequest;

        private FacebookUserData _fbUserData;

        [SetUp]
        public void Setup()
        {
            // Query to facebook api
            UserInfoUrl = String.Format(@"https://graph.facebook.com/v2.0/me?access_token={0}&fields=id,first_name,last_name,name,gender,email,birthday,timezone,location,picture.type(large)",
                    _accessToken);

            // Object that we will make Json
            _fbUserData = new FacebookUserData
            {
                id = "12345",
                first_name = "Beseda",
                last_name = "Dmitrij",
                name = "Beseda Dmitrij",
                gender = "male",
                email = "besedadg@gmail.com",
                birthday = "12/1/1992",
                timezone = 2,
                location = new FacebookUserData.Location {id = "1", name = "Donetsk, Ukraine"},
                picture = new FacebookUserData.Picture
                {
                    data = new FacebookUserData.Data
                    {
                        url = "photo.jpg"
                    }
                },
            };

            _userToJson = JsonConvert.SerializeObject(_fbUserData);

            _webRequest = Substitute.For<IWebClient>();
            _webRequest.GetWebData(UserInfoUrl).Returns(Task.FromResult(_userToJson));
        }

        [Test]
        public async void Get_Facebook_User_Data()
        {
            // Arranage
            User expectedUser = (User) _fbUserData;
            // Act
            var provider = new FacebookAccountProvider(_webRequest, new UserCredential
            {
                SocialNetworkId = _userId,
                AccessToken = _accessToken
            });
            var user = await provider.GetUserInfoAsync("besedadg@gmail.com");
            // Assert
            user.ToString().Should().Be(expectedUser.ToString(), "");
        }
    }
}
