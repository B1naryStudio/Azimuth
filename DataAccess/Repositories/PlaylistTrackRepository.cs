
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;

namespace Azimuth.DataAccess.Repositories
{
    public class PlaylistTrackRepository : BaseRepository<PlaylistTrack>
    {
        public PlaylistTrackRepository(ISession session)
            : base(session)
        {
        }
    }
}
