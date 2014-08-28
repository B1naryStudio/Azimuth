using System.Collections.Generic;
using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;
using NHibernate.Linq;
using TweetSharp;

namespace Azimuth.DataAccess.Repositories
{
    public class PlaylistRepository : BaseRepository<Playlist>
    {
        public PlaylistRepository(ISession session) : base(session)
        {
        }
    }
}