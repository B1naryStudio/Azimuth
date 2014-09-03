using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Services.Concrete;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace Azimuth.Infrastructure.Concrete
{
    public class SocialNetworkApiModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMusicServiceFactory>().ToFactory();
            Bind<IMusicServiceWorkUnit>().To<MusicServiceWorkUnit>();
            Bind<IUserTracksService>().To<UserTracksService>();
            Bind<ISocialNetworkApi>().To<VkApi>().Named("Vkontakte");
            Bind<IMusicService<LastfmTrackData>>().To<LastfmApi>();
            Bind<IMusicService<DeezerTrackData.TrackData>>().To<DeezerApi>();
            Bind<IMusicService<string[]>>().To<ChartLyricsApi>();
            //Bind<ISocialNetworkApi>().To<FacebookApi>().Named("Facebook");
            //Bind<ISocialNetworkApi>().To<TwitterApi>().Named("Twitter");
            //Bind<ISocialNetworkApi>().To<GoogleApi>().Named("Google");
        }
    }
    public interface IMusicServiceFactory
    {
        IMusicService<T> Resolve<T>();
    }
}