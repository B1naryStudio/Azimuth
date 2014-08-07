
using System;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TweetSharp;

namespace Azimuth.Tests
{
    [TestFixture]
    public class TwitterAccountProviderTests
    {
        // Data for webrequest
        private const string _accessToken = "some access token";
        private const string _accessSecret = "some access secret";
        private const string _consumerKey = "some consumer key";
        private const string _consumerSecret = "some consumer secret";
        private const string _socialNetwork = "Twitter";
        private const string _userId = "1234567";

        private TwitterUser _user;

        private IWebClient _webClient;

        [SetUp]
        public void Setup()
        {
            _user = new TwitterUser
            {
                Name = "Azimuth Project",
                Location = "Donetsk, Ukraine",
                ScreenName = "AzimuthP",
                ProfileImageUrl = "photo.jpg"
            };

            _webClient = Substitute.For<IWebClient>();
            _webClient.GetWebData(_consumerKey, _consumerSecret, _accessToken, _accessSecret).Returns(Task.FromResult(_user));
        }

        [Test]
        public async void Get_Twitter_User_Data()
        {
            // Arrange
            //User expectedUser = new User
            //{
            //    Name = new Name {FirstName = "Azimuth Project", LastName = String.Empty},
            //    Birthday = String.Empty,
            //    Email = String.Empty,
            //    Gender = String.Empty,
            //    Location = new Location {City = "Donetsk, Ukraine", Country = String.Empty},
            //    Timezone = -100,
            //    ScreenName = "AzimuthP",
            //    Photo = "photo.jpg"
            //};

            User expectedUser = (User) _user;

            // Act
            IAccountProvider provider = new TwitterAccountProvider(_webClient, _userId, _accessToken, _accessSecret,
                _consumerKey, _consumerSecret);
            User currentUser = await provider.GetUserInfoAsync();
            //Assert
            currentUser.ToString().Should().Be(expectedUser.ToString());
        }
    }
}
