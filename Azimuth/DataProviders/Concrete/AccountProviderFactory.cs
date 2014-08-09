using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using Ninject;
using Ninject.Parameters;

namespace Azimuth.DataProviders.Concrete
{
    public static class AccountProviderFactory
    {
        public static IAccountProvider GetAccountProvider(UserCredential cred)
        {
            var userCredential = new ConstructorArgument("userCredential", cred);

            return MvcApplication.Container.Get<IAccountProvider>(cred.SocialNetworkName, userCredential);
        }
    }
}