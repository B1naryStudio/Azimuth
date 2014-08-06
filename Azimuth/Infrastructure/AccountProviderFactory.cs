using Ninject;
using Ninject.Parameters;

namespace Azimuth.Infrastructure
{
    public static class AccountProviderFactory
    {
        public static IAccountProvider GetAccountProvider(string socialNetwork, string userId, string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret)
        {
            var userIdParam = new ConstructorArgument("userId", userId);
            var accessTokenParam = new ConstructorArgument("accessToken", accessToken);
            var accessTokenSecretParam = new ConstructorArgument("accessTokenSecret", accessTokenSecret);
            var consumerKeyParam = new ConstructorArgument("consumerKey", consumerKey);
            var consumerSecretParam = new ConstructorArgument("consumerSecret", consumerSecret);

            var webClient = new ConstructorArgument("webClient", MvcApplication.Container.Get<IWebClient>());

            return MvcApplication.Container.Get<IAccountProvider>(socialNetwork, webClient, userIdParam, accessTokenParam, accessTokenSecretParam, consumerKeyParam, consumerSecretParam);
        }
    }
}