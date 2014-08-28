using System;
using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure.Concrete;
using FluentAssertions;
using Ninject;
using Ninject.Parameters;
using NUnit.Framework;

namespace Azimuth.Tests
{
    [TestFixture]
    internal class AccountProviderFactoryTests
    {
        private IKernel _kernel;

        private string _socialNetwork;
        private string _accessToken;

        private ConstructorArgument _userCredentialsParam;

        private UserCredential _userCredentials;

        [SetUp]
        public void Setup()
        {
            _kernel = new StandardKernel(new AccountProviderModule());

            _accessToken = "some access token";

            _userCredentials = new UserCredential
            {
                Email = "some email",
                SocialNetworkId = "some user id in social network",
                AccessToken = _accessToken,
                SocialNetworkName = "some social network",
                AccessTokenExpiresIn = "24:00:00",
                ConsumerSecret = "some consumer secret",
                ConsumerKey = "some consumer key",
                AccessTokenSecret = "some access token secret"
            };

            _userCredentialsParam = new ConstructorArgument("userCredential", _userCredentials);
        }

        [Test]
        public void Get_Facebook_Account_Provider_Instance()
        {
            // Arrange
            _socialNetwork = "Facebook";
            // Act
            var provider = _kernel.Get<IAccountProvider>(_socialNetwork, _userCredentialsParam);
            // Assert
            provider.Should().BeOfType<FacebookAccountProvider>("we asked account provider instance for facebook");
        }

        [Test]
        public void Get_Facebook_Account_provider_with_wrong_input_arguments()
        {
            // Arrange
            _socialNetwork = "Facebook";
            // Act
            // Assert
            _kernel.Invoking(g => g.Get<IAccountProvider>())
                .ShouldThrow<ActivationException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>()\n");
            _kernel.Invoking(g => g.Get<IAccountProvider>(_socialNetwork))
                .ShouldThrow<ArgumentException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>(one param)\n");
            _kernel.Invoking(g => g.Get<IAccountProvider>(_userCredentialsParam)).ShouldThrow<ActivationException>("wrong params order");
        }

        [Test]
        public void Get_VK_Account_Provider_Instance()
        {
            //Arange
            _socialNetwork = "Vkontakte";
            //Act
            var provider = _kernel.Get<IAccountProvider>(_socialNetwork, _userCredentialsParam);
            //Assert
            provider.Should().BeOfType<VkAccountProvider>("we asked account provider instance for vkontakte");
        }

        [Test]
        public void Get_Vkontakte_Account_provider_with_wrong_input_arguments()
        {
            // Arrange
            _socialNetwork = "Vkontakte";
            // Act
            //Assert
            _kernel.Invoking(g => g.Get<IAccountProvider>())
                .ShouldThrow<ActivationException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>()\n");
            _kernel.Invoking(g => g.Get<IAccountProvider>(_socialNetwork))
                .ShouldThrow<ArgumentException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>(one param)\n");
            _kernel.Invoking(g => g.Get<IAccountProvider>(_userCredentialsParam)).ShouldThrow<ActivationException>("wrong params order");
        }

        [Test]
        public void Get_Twitter_Account_Provider_Instance()
        {
            // Arrange
            _socialNetwork = "Twitter"; 
            var userCredentialParam = new ConstructorArgument("userCredential", _userCredentials);
            //Act
            var provider = _kernel.Get<IAccountProvider>(_socialNetwork, userCredentialParam);
            // Assert
            provider.Should()
                .BeOfType<TwitterAccountProvider>("we asked account providerinstance for twitter");
        }

        [Test]
        public void Get_Twitter_Account_provider_with_wrong_input_arguments()
        {
            // Arrange
            _socialNetwork = "Twitter";
            // Act
            //Assert
            _kernel.Invoking(g => g.Get<IAccountProvider>())
                .ShouldThrow<ActivationException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>()\n");
            _kernel.Invoking(g => g.Get<IAccountProvider>(_socialNetwork))
                .ShouldThrow<ArgumentException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>(one param)\n");
            _kernel.Invoking(g => g.Get<IAccountProvider>(_userCredentialsParam)).ShouldThrow<ActivationException>("wrong params order");
        }

        [Test]
        public void Get_Google_Account_Provider_Instance()
        {
            // Arrange
            _socialNetwork = "Google";
            var userCredentialParam = new ConstructorArgument("userCredential", _userCredentials);
            //Act
            var provider = _kernel.Get<IAccountProvider>(_socialNetwork, userCredentialParam);
            //Assert
            provider.Should()
                .BeOfType<GoogleAccountProvider>("we asked account provider instance for google");

        }

        [Test]
        public void Get_Google_Account_provider_with_wrong_input_arguments()
        {
            // Arrange
            _socialNetwork = "Google";
            // Act
            //Assert
            _kernel.Invoking(g => g.Get<IAccountProvider>())
                .ShouldThrow<ActivationException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>()\n");
            _kernel.Invoking(g => g.Get<IAccountProvider>(_socialNetwork))
                .ShouldThrow<ArgumentException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>(one param)\n");
            _kernel.Invoking(g => g.Get<IAccountProvider>(_userCredentialsParam)).ShouldThrow<ActivationException>("wrong params order");
        }
    }
}
