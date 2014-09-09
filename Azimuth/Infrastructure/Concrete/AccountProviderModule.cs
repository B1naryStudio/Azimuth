using Azimuth.ApiControllers;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure.Interfaces;
using Azimuth.Services.Concrete;
using Azimuth.Services.Interfaces;
using Ninject.Modules;

namespace Azimuth.Infrastructure.Concrete
{
    public class AccountProviderModule:NinjectModule
    {
        public override void Load()
        {
            Bind<IAccountService>().To<AccountService>();
            Bind<ISettingsService>().To<SettingsService>();
            Bind<IPlaylistService>().To<PlaylistService>();
            Bind<IUserService>().To<UserService>();
            Bind<IListenersService>().To<ListenersService>();
            Bind<IPlaylistLikesService>().To<PlaylistLikesService>();
            Bind<IPlaylistListenedService>().To<PlaylistListenedService>();
            Bind<INotificationService>().To<NotificationService>();
            Bind<IWebClient>().To<WebClient>();
            Bind<IAccountProvider>().To<FacebookAccountProvider>().Named("Facebook");
            Bind<IAccountProvider>().To<VkAccountProvider>().Named("Vkontakte");
            Bind<IAccountProvider>().To<TwitterAccountProvider>().Named("Twitter");
            Bind<IAccountProvider>().To<GoogleAccountProvider>().Named("Google");
            //Bind<IChartLyricsApi>().To<ChartLyricsApi>();
        }
    }
}