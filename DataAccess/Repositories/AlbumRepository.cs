using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;

namespace Azimuth.DataAccess.Repositories
{
    public class AlbumRepository : BaseRepository<Album>
    {
        public AlbumRepository(ISession session) : base(session)
        {
        }
    }
}