using System;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataProviders.Concrete;
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
            const string liveCoord = "40,40";
            //const string liveCoord = "40,30";
            _googleUserData = new GoogleUserData
            {
                Name = new GoogleUserData.GoogleName
                {
                    GivenName = "Ivan",
                    FamilyName = "Bakun"
                },
                DisplayName = "Ivan Bakun",
                Gender = "male",
                Emails = new []{new GoogleUserData.Email
                {
                    Type = "account",
                    Value = "ivanbakun@gmail.com"
                }},
                Birthday = "26/1/1994",
                PlacesLived = new []{new GoogleUserData.GoogleLocation
                {
                    Primary = true,
                    Value = "Donetsk, Ukraine" 
                } },
                Image = new GoogleUserData.Photo
                {
                    Url = "photo.jpg"
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
                RawOffset = 3600
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
            var expectedUser = Mapper.Map(_googleUserData, new User());
            expectedUser.Timezone = _googleTimeZone.RawOffset/3600;
            // Act
            var provider = new GoogleAccountProvider(_webRequest, new UserCredential
            {
                SocialNetworkId = _userId,
                AccessToken = _accessToken
            });
            var user = await provider.GetUserInfoAsync();
            // Assert
            user.ToString().Should().Be(expectedUser.ToString());
        }

        [Test]
        public async void GetGoogleUserWithoutLocation()
        {
            // Arrange
            var placesLived = _googleUserData.PlacesLived;
            _googleUserData.PlacesLived = new GoogleUserData.GoogleLocation[]{ };//Delete location from googleacc
            var expectedUser = Mapper.Map(_googleUserData, new User());
            // Act
            var provider = new GoogleAccountProvider(_webRequest, new UserCredential
            {
                SocialNetworkId = _userId,
                AccessToken = _accessToken
            });
            var user = await provider.GetUserInfoAsync();
            // Assert
            user.ToString().Should().Be(expectedUser.ToString());
            _googleUserData.PlacesLived = placesLived;
        }

        [Test]
        public async void GetGoogleUserWithOnlyCity()
        {
            // Arrange
            var placesLived = _googleUserData.PlacesLived;
            _googleUserData.PlacesLived[0].Value = _googleUserData.PlacesLived[0].Value.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            var expectedUser = Mapper.Map(_googleUserData, new User());
            expectedUser.Timezone = _googleTimeZone.RawOffset / 3600;
            // Act
            var provider = new GoogleAccountProvider(_webRequest, new UserCredential
            {
                SocialNetworkId = _userId,
                AccessToken = _accessToken
            });
            var user = await provider.GetUserInfoAsync();
            // Assert
            user.ToString().Should().Be(expectedUser.ToString());
            _googleUserData.PlacesLived = placesLived;
        }
    }
}
