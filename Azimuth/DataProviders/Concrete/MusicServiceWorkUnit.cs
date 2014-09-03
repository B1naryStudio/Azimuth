
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure.Concrete;

namespace Azimuth.DataProviders.Concrete
{
    public class MusicServiceWorkUnit : IMusicServiceWorkUnit
    {
        private readonly IMusicServiceFactory _musicServiceFactory;
        public MusicServiceWorkUnit(IMusicServiceFactory musicServiceFactory)
        {
            _musicServiceFactory = musicServiceFactory;
        }
        public IMusicService<T> GetMusicService<T>()
        {
            return _musicServiceFactory.Resolve<T>();
        }
    }
}