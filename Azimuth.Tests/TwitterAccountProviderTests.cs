using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataProviders.Concrete;
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
        private const string AccessToken = "some access token";
        private const string AccessSecret = "some access secret";
        private const string ConsumerKey = "some consumer key";
        private const string ConsumerSecret = "some consumer secret";
        private const string UserId = "1234567";

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
            _webClient.GetWebData(ConsumerKey, ConsumerSecret, AccessToken, AccessSecret).Returns(Task.FromResult(_user));
        }

        [Test]
        public async void Get_Twitter_User_Data()
        {
            // Arrange
            var expectedUser = Mapper.Map(_user, new User());
            // Act
            var provider = new TwitterAccountProvider(_webClient, new UserCredential
            {
                AccessToken = AccessToken,
                AccessTokenSecret = AccessSecret,
                ConsumerKey = ConsumerKey,
                ConsumerSecret = ConsumerSecret,
                SocialNetworkId = UserId
            });
            var currentUser = await provider.GetUserInfoAsync();
            //Assert
            currentUser.ToString().Should().Be(expectedUser.ToString());
        }
    }
}
