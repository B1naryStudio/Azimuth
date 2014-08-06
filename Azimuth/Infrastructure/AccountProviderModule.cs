using Ninject.Extensions.Factory;
using Ninject.Modules;
using Ninject.Parameters;

namespace Azimuth.Infrastructure
{
    public class AccountProviderModule:NinjectModule
    {
        public override void Load()
        {
            Bind<IAccountService>().To<AccountService>();
            Bind<IAccountProvider>().To<FacebookAccountProvider>().Named("Facebook");
            Bind<IAccountProvider>().To<VKAccountProvider>().Named("Vkontakte");
            Bind<IAccountProvider>().To<TwitterAccountProvider>().Named("Twitter");
            Bind<IAccountProvider>().To<GoogleAccountProvider>().Named("Google");
        }
    }
}