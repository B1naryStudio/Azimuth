using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
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
        // Classes for write JsonSerialization

        class Data
        {
            public bool is_silhouette { get; set; }
            public string url { get; set; }
        }

        class Picture
        {
            public Data data { get; set; }
        }

        class RootObject
        {
            public GoogleName name { get; set; }
            public GoogleLocation[] placesLived { get; set; }
            public Photo image { get; set; }
            public string birthday { get; set; }
            public string gender { get; set; }
            public string displayName { get; set; }
            public Email[] emails { get; set; }
        }

        class TimeZone
        {
            public int rawOffset { get; set; }
        }

        // Fields for tests
        private string _userToJson;

        private string _accessToken;
        private string _userId;

        //private ConstructorArgument _accessTokenParam;
        //private ConstructorArgument _userIdParam;

        public string UserInfoUrl { get; set; }

        private IWebClient _webRequest;

        private RootObject _googleUserData;
        private TimeZone _googleTimeZone;

        [SetUp]
        public void Setup()
        {
            // Parametres for calling GetUSerInfoAsync
            _accessToken = "some access token";
            _userId = "some user id";

            //_accessTokenParam = new ConstructorArgument("accessToken", _accessToken);
            //_userIdParam = new ConstructorArgument("userId", _userId);

            // Query to facebook api
            UserInfoUrl = String.Format(
                @"https://www.googleapis.com/plus/v1/people/{0}?access_token={1}&fields=birthday%2CdisplayName%2Cemails%2Cgender%2Cimage%2Cname(familyName%2CgivenName)%2CplacesLived",
                _userId,
                _accessToken);

            // Object that we will make Json
            const string livePlace = "Donetsk, Ukraine";
            const string liveCoord = "40,30";
            _googleUserData = new RootObject
            {
                name = new GoogleName
                {
                    givenName = "Ivan",
                    familyName = "Bakun"
                },
                displayName = "Ivan Bakun",
                gender = "male",
                emails = new []{new Email
                {
                    type = "account",
                    value = "ivanbakun@gmail.com"
                }},
                birthday = "26/1/1994",
                placesLived = new []{new GoogleLocation
                {
                    primary = true,
                    value = "Donetsk, Ukraine" 
                } },
                image = new Photo
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
            _googleTimeZone = new TimeZone
            {
                rawOffset = 3600
            };
            _userToJson = JsonConvert.SerializeObject(_googleUserData);
            var locatioToJson = JsonConvert.SerializeObject(coordinates);
            var timezoneToJson = JsonConvert.SerializeObject(_googleTimeZone);
            var userCoordUrl = String.Format(
                    @"https://maps.googleapis.com/maps/api/geocode/json?address={0}", livePlace);
            var userTimezoneUrl = String.Format(
                        @"https://maps.googleapis.com/maps/api/timezone/json?location={0}&timestamp=1331161200",
                        liveCoord);
            _webRequest = Substitute.For<IWebClient>();
            _webRequest.GetWebData(UserInfoUrl).Returns(Task.FromResult(_userToJson));
            _webRequest.GetWebData(userCoordUrl).Returns(Task.FromResult(locatioToJson));
            _webRequest.GetWebData(userTimezoneUrl).Returns(Task.FromResult(timezoneToJson));
        }

        [Test]
        public async void Get_Google_User_Data()
        {
            // Arrange
            var expectedUser = new User
            {
                Name = new Name
                {
                    FirstName = _googleUserData.name.givenName,
                    LastName = _googleUserData.name.familyName
                },
                ScreenName = _googleUserData.displayName,
                Gender = _googleUserData.gender,
                Email = _googleUserData.emails[0].value,
                Birthday = _googleUserData.birthday,
                Timezone = _googleTimeZone.rawOffset/3600,
                Location = new DataAccess.Entities.Location
                {
                    City = _googleUserData.placesLived[0].value.Split(',').First(),
                    Country = _googleUserData.placesLived[0].value.Split(' ').Last()
                },
                Photo = _googleUserData.image.url
            };
            // Act
            var provider = new GoogleAccountProvider(_webRequest, _userId, _accessToken);
            var user = await provider.GetUserInfoAsync();
            // Assert
            user.ToString().Should().Be(expectedUser.ToString(), "");
        }
    }
}
