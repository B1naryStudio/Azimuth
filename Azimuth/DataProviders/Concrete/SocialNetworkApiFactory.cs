using Azimuth.DataProviders.Interfaces;
using Ninject;
using Ninject.Parameters;

namespace Azimuth.DataProviders.Concrete
{
    public class SocialNetworkApiFactory
    {
        public static ISocialNetworkApi GetSocialNetworkApi(string provider)
        {
            return MvcApplication.Container.Get<ISocialNetworkApi>(provider);
        }
    }
}