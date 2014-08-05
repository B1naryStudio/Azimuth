using Ninject;
using Ninject.Parameters;

namespace Azimuth.Infrastructure
{
    public static class AccountProviderFactory
    {
        private static readonly IKernel _kernel;

        static AccountProviderFactory()
        {
            _kernel = new StandardKernel(new AccountProviderModule());
        }

        public static IAccountProvider GetAccountProvider(string socialNetwork, string userId, string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret)
        {
            var userIdParam = new ConstructorArgument("userId", userId);
            var accessTokenParam = new ConstructorArgument("accessToken", accessToken);
            var accessTokenSecretParam = new ConstructorArgument("accessTokenSecret", accessTokenSecret);
            var consumerKeyParam = new ConstructorArgument("consumerKey", consumerKey);
            var consumerSecretParam = new ConstructorArgument("consumerSecret", consumerSecret);

            return _kernel.Get<IAccountProvider>(socialNetwork, userIdParam, accessTokenParam, accessTokenSecretParam, consumerKeyParam, consumerSecretParam);
        }
    }
}