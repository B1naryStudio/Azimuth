using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Ninject.Modules;

namespace Azimuth.Infrastructure
{
    public class AccountProviderModule:NinjectModule
    {
        public override void Load()
        {
            Bind<IAccountService>().To<AccountService>();
            Bind<IWebClient>().To<WebClient>();
            Bind<IVkApi>().To<VkApi>();
            Bind<IAccountProvider>().To<FacebookAccountProvider>().Named("Facebook");
            Bind<IAccountProvider>().To<VKAccountProvider>().Named("Vkontakte");
            Bind<IAccountProvider>().To<TwitterAccountProvider>().Named("Twitter");
            Bind<IAccountProvider>().To<GoogleAccountProvider>().Named("Google");
        }
    }
}