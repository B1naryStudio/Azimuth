using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using Azimuth.Infrastructure.Concrete;
using Ninject;
using Ninject.Parameters;

namespace Azimuth.DataProviders.Concrete
{
    public static class AccountProviderFactory
    {
        public static IAccountProvider GetAccountProvider(UserCredential credential)
        {
            var userCredential = new ConstructorArgument("userCredential", credential);

            return MvcApplication.Container.Get<IAccountProvider>(credential.SocialNetworkName, userCredential);
        }
    }
}