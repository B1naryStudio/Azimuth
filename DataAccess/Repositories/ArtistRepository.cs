using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;

namespace Azimuth.DataAccess.Repositories
{
    public class ArtistRepository : BaseRepository<Artist>
    {
        public ArtistRepository(ISession session) : base(session)
        {
        }
    }
}