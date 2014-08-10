using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;

namespace Azimuth.DataAccess.Repositories
{
    public class PlaylistRepository : BaseRepository<Playlist>
    {
        public PlaylistRepository(ISession session) : base(session)
        {
        }
    }
}