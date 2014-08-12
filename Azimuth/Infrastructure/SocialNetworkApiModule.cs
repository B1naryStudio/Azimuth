using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Services;
using Ninject.Modules;

namespace Azimuth.Infrastructure
{
    public class SocialNetworkApiModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUserTracksService>().To<UserTracksService>();
            Bind<ISocialNetworkApi>().To<VkApi>().Named("Vkontakte");
        }
    }
}