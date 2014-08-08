using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using FluentAssertions;
using Ninject;
using Ninject.Parameters;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    class AccountProviderFactoryTests
    {
        private IKernel _kernel;

        private string _socialNetwork;
        private string _userId;
        private string _accessToken;
        private string _accessTokenSecret;
        private string _consumerKey;
        private string _consumerSecret;

        private ConstructorArgument _userIdParam;
        private ConstructorArgument _accessTokenParam;
        private ConstructorArgument _accessTokenSecretParam;
        private ConstructorArgument _consumerKeyParam;
        private ConstructorArgument _consumerKeySecretParam;

        [SetUp]
        public void Setup()
        {
            _kernel = new StandardKernel(new AccountProviderModule());

            _userId = "some user id";
            _accessToken = "some access token";
            _accessTokenSecret = "some access token secret";
            _consumerKey = "some consumer key";
            _consumerSecret = "some consumer key secret";

            _userIdParam = new ConstructorArgument("userId", _userId);
            _accessTokenParam = new ConstructorArgument("accessToken", _accessToken);
            _accessTokenSecretParam = new ConstructorArgument("accessTokenSecret", _accessTokenSecret);
            _consumerKeyParam = new ConstructorArgument("consumerKey", _consumerKey);
            _consumerKeySecretParam = new ConstructorArgument("consumerSecret", _consumerSecret);
        }

        [Test]
        public void Get_Facebook_Account_Provider_Instance()
        {
            // Arrange
            _socialNetwork = "Facebook";
            // Act
            IAccountProvider fbAccountProvider = _kernel.Get<IAccountProvider>(_socialNetwork, _userIdParam, _accessTokenParam);
            // Assert
            fbAccountProvider.Should().BeOfType<FacebookAccountProvider>("we asked account provider instance for facebook");
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
                .ShouldThrow<ActivationException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>(one param)\n");
            _kernel.Invoking(g => g.Get<IAccountProvider>(_socialNetwork, _userIdParam))
                .ShouldThrow<ActivationException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>(two params)\n");
        }

        [Test]
        public void Get_VK_Account_Provider_Instance()
        {
            //Arange
            _socialNetwork = "Vkontakte";
            //Act
            IAccountProvider vkAccountProvider = _kernel.Get<IAccountProvider>(_socialNetwork, _userIdParam, _accessTokenParam);
            //Assert
            vkAccountProvider.Should().BeOfType<VKAccountProvider>("we asked account provider instance for vkontakte");
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
                .ShouldThrow<ActivationException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>(one param)\n");
            _kernel.Invoking(g => g.Get<IAccountProvider>(_socialNetwork, _userIdParam))
                .ShouldThrow<ActivationException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>(two params)\n");
        }

        [Test]
        public void Get_Twitter_Account_Provider_Instance()
        {
            // Arrange
            _socialNetwork = "Twitter";
            // Act
            IAccountProvider twitterAccountProvider = _kernel.Get<IAccountProvider>(_socialNetwork, _userIdParam, _accessTokenParam, _accessTokenSecretParam, _consumerKeyParam, _consumerKeySecretParam);
            // Assert
            twitterAccountProvider.Should()
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
                .ShouldThrow<ActivationException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>(one param)\n");
            _kernel.Invoking(g => g.Get<IAccountProvider>(_socialNetwork, _userIdParam))
                .ShouldThrow<ActivationException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>(two params)\n");
        }

        [Test]
        public void Get_Google_Account_Provider_Instance()
        {
            // Arrange
            _socialNetwork = "Google";
            //Act
            IAccountProvider googleAccountProvider = _kernel.Get<IAccountProvider>(_socialNetwork, _userIdParam, _accessTokenParam);
            //Assert
            googleAccountProvider.Should()
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
                .ShouldThrow<ActivationException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>(one param)\n");
            _kernel.Invoking(g => g.Get<IAccountProvider>(_socialNetwork, _userIdParam))
                .ShouldThrow<ActivationException>(
                    "need _kernel.Get<IAccountProvider>(three input params)  but was _kernel.Get<IAccountProvider>(two params)\n");
        }
    }
}
