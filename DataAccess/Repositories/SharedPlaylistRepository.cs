using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;

namespace Azimuth.DataAccess.Repositories
{
    public class SharedPlaylistRepository : BaseRepository<SharedPlaylist>
    {
        public SharedPlaylistRepository(ISession session) : base(session)
        {
        }    
    }
}