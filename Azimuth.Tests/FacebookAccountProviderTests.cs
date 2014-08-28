using System;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataProviders.Concrete;
using Azimuth.Infrastructure;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Infrastructure.Interfaces;
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
        private const string AccessToken = "some access token";
        private const string UserId = "some user id";

        public string UserInfoUrl { get; set; }

        private IWebClient _webRequest;

        private FacebookUserData _fbUserData;

        [SetUp]
        public void Setup()
        {
            // Query to facebook api
            UserInfoUrl = String.Format(@"https://graph.facebook.com/v2.0/me?access_token={0}&fields=id,first_name,last_name,name,gender,email,birthday,timezone,location,picture.type(large)",
                    AccessToken);

            // Object that we will make Json
            _fbUserData = new FacebookUserData
            {
                Id = "12345",
                FirstName = "Beseda",
                LastName = "Dmitrij",
                Name = "Beseda Dmitrij",
                Gender = "male",
                Email = "besedadg@gmail.com",
                Birthday = "12/1/1992",
                Timqzone = 2,
                Location = new FacebookUserData.FBLocation() {Id = "1", Name = "Donetsk, Ukraine"},
                Picture = new FacebookUserData.FBPicture()
                {
                    Data = new FacebookUserData.Data
                    {
                        Url = "photo.jpg"
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
            var expectedUser = Mapper.Map(_fbUserData, new User());
            // Act
            var provider = new FacebookAccountProvider(_webRequest, new UserCredential
            {
                SocialNetworkId = UserId,
                AccessToken = AccessToken
            });
            var user = await provider.GetUserInfoAsync("besedadg@gmail.com");
            // Assert
            user.ToString().Should().Be(expectedUser.ToString());
        }
    }
}
