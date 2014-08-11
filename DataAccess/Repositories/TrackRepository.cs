using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;

namespace Azimuth.DataAccess.Repositories
{
    public class TrackRepository : BaseRepository<Track>
    {
        public TrackRepository(ISession session) : base(session)
        {
        }
    }
}