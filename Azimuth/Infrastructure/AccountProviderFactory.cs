using Ninject;
using Ninject.Parameters;

namespace Azimuth.Infrastructure
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