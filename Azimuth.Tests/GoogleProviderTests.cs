using System;
using System.Linq;
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
    public class GoogleAccountProviderTests
    {
        // Fields for tests
        private string _userToJson;

        private string _accessToken;
        private string _userId;

        public string UserInfoUrl { get; set; }

        private IWebClient _webRequest;

        private GoogleUserData _googleUserData;
        private GoogleUserData.Timezone _googleTimeZone;

        [SetUp]
        public void Setup()
        {
            // Parametres for calling GetUSerInfoAsync
            _accessToken = "some access token";
            _userId = "some user id";

            // Query to facebook api
            UserInfoUrl = String.Format(
                @"https://www.googleapis.com/plus/v1/people/{0}?access_token={1}&fields=birthday%2CdisplayName%2Cemails%2Cgender%2Cimage%2Cname(familyName%2CgivenName)%2CplacesLived",
                _userId,
                _accessToken);

            // Object that we will make Json
            const string livePlace = "Donetsk, Ukraine";
            const string liveCity = "Donetsk";
            const string liveCoord = "40,30";
            _googleUserData = new GoogleUserData
            {
                name = new GoogleUserData.GoogleName
                {
                    givenName = "Ivan",
                    familyName = "Bakun"
                },
                displayName = "Ivan Bakun",
                gender = "male",
                emails = new []{new GoogleUserData.Email
                {
                    type = "account",
                    value = "ivanbakun@gmail.com"
                }},
                birthday = "26/1/1994",
                placesLived = new []{new GoogleUserData.GoogleLocation
                {
                    primary = true,
                    value = "Donetsk, Ukraine" 
                } },
                image = new GoogleUserData.Photo
                {
                    url = "photo.jpg"
                }
            };
            var coordinates = new
            {
                results = new[]
                {
                    new
                    {
                        geometry = new
                        {
                            location = new
                            {
                                lat = liveCoord.Split(',')[0],
                                lng = liveCoord.Split(',')[1]
                            }
                        }
                    }
                }
            };
            _googleTimeZone = new GoogleUserData.Timezone
            {
                rawOffset = 3600
            };
            _userToJson = JsonConvert.SerializeObject(_googleUserData);
            var locatioToJson = JsonConvert.SerializeObject(coordinates);
            var timezoneToJson = JsonConvert.SerializeObject(_googleTimeZone);
            var userCoordUrl1 = String.Format(
                    @"https://maps.googleapis.com/maps/api/geocode/json?address={0}", livePlace);
            var userCoordUrl2 = String.Format(
                    @"https://maps.googleapis.com/maps/api/geocode/json?address={0}", liveCity);
            var userTimezoneUrl = String.Format(
                        @"https://maps.googleapis.com/maps/api/timezone/json?location={0}&timestamp=1331161200",
                        liveCoord);
            _webRequest = Substitute.For<IWebClient>();
            _webRequest.GetWebData(UserInfoUrl).Returns(x =>
            {
                _userToJson = JsonConvert.SerializeObject(_googleUserData);
                return Task.FromResult(_userToJson);
            });
            _webRequest.GetWebData(userCoordUrl1).Returns(Task.FromResult(locatioToJson));
            _webRequest.GetWebData(userCoordUrl2).Returns(Task.FromResult(locatioToJson));
            _webRequest.GetWebData(userTimezoneUrl).Returns(Task.FromResult(timezoneToJson));
        }

        [Test]
        public async void Get_Google_User_Data()
        {
            // Arrange
            var expectedUser = (User) _googleUserData;
            expectedUser.Timezone = _googleTimeZone.rawOffset/3600;
            // Act
            var provider = new GoogleAccountProvider(_webRequest, new UserCredential
            {
                SocialNetworkId = _userId,
                AccessToken = _accessToken
            });
            var user = await provider.GetUserInfoAsync();
            // Assert
            user.ToString().Should().Be(expectedUser.ToString(), "");
        }

        [Test]
        public async void GetGoogleUserWithoutLocation()
        {
            // Arrange
            var placesLived = _googleUserData.placesLived;
            _googleUserData.placesLived = new GoogleUserData.GoogleLocation[]{ };//Delete location from googleacc
            var expectedUser = (User)_googleUserData;
            // Act
            var provider = new GoogleAccountProvider(_webRequest, new UserCredential
            {
                SocialNetworkId = _userId,
                AccessToken = _accessToken
            });
            var user = await provider.GetUserInfoAsync();
            // Assert
            user.ToString().Should().Be(expectedUser.ToString(), "");
            _googleUserData.placesLived = placesLived;
        }

        [Test]
        public async void GetGoogleUserWithOnlyCity()
        {
            // Arrange
            var placesLived = _googleUserData.placesLived;
            _googleUserData.placesLived[0].value = _googleUserData.placesLived[0].value.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            var expectedUser = (User)_googleUserData;
            expectedUser.Timezone = _googleTimeZone.rawOffset / 3600;
            // Act
            var provider = new GoogleAccountProvider(_webRequest, new UserCredential
            {
                SocialNetworkId = _userId,
                AccessToken = _accessToken
            });
            var user = await provider.GetUserInfoAsync();
            // Assert
            user.ToString().Should().Be(expectedUser.ToString(), "");
            _googleUserData.placesLived = placesLived;
        }
    }
}
