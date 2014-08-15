using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataProviders.Concrete;
using Azimuth.Infrastructure;
using Azimuth.Shared.Dto;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Azimuth.Tests
{
    [TestFixture]
    class VKAccountProviderTests
    {

        // Fields for tests
        private string _userToJson;

        private const string AccessToken = "some user id";
        private const string UserId = "some access token";
        private const string Email = "besedadg@gmail.com";

        public string UserInfoUrl { get; set; }

        private IWebClient _webRequest;

        private VKUserData.Response _vkUserData;

        [SetUp]
        public void SetUp()
        {
            // Query to VK api
            UserInfoUrl = String.Format(
                @"https://api.vk.com/method/users.get?user_id={0}&fields=screen_name,bdate,sex,city,country,photo_max_orig,timezone&v=5.23&access_token={1}",
                UserId,
                AccessToken);

            // Object that we will make Json
            _vkUserData = new VKUserData.Response
            {
                response = new List<VKUserData>
                {
                    new VKUserData
                    {
                        first_name = "Beseda",
                        last_name = "Dmitrij",
                        screen_name = "Beseda Dmitrij",
                        sex = VKUserData.Sex.male,
                        bdate = "12/1/1992",
                        city = new VKUserData.City{id = 1, title = "Donetsk"},
                        country = new VKUserData.Country {id = 1, title = "Ukraine"},
                        timezone = 2,
                        photo_max_orig = "photo.jpg",
                        email = Email
                    }
                }
            };

            // Make Json
            _userToJson = JsonConvert.SerializeObject(_vkUserData);

            _webRequest = Substitute.For<IWebClient>();
            _webRequest.GetWebData(UserInfoUrl).Returns(Task.FromResult(_userToJson));
        }

        [Test]
        public async void Get_VK_User_Data()
        {
            // Arrange
            var expectedUser = Mapper.Map(_vkUserData, new User());
            // Act
            var provider = new VKAccountProvider(_webRequest, new UserCredential
            {
                SocialNetworkId = UserId,
                AccessToken = AccessToken
            });
            var user = await provider.GetUserInfoAsync(Email);
            // Assert
            user.ToString().Should().Be(expectedUser.ToString());
        }
    }
}
