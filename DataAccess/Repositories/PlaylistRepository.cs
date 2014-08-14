using System.Collections.Generic;
using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;
using NHibernate.Linq;

namespace Azimuth.DataAccess.Repositories
{
    public class PlaylistRepository : BaseRepository<Playlist>
    {
        public PlaylistRepository(ISession session) : base(session)
        {
        }

        public IEnumerable<Playlist> GetByCreatorId(long id)
        {
            return _session.Query<Playlist>().Where(pl => pl.Creator.Id == id);
        }
    }
}