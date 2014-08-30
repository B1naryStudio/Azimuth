using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;

namespace Azimuth.DataAccess.Repositories
{
    class AlbumListenersRepository : BaseRepository<AlbumListeners>
    {
        public AlbumListenersRepository(ISession session)
            : base(session)
        {
        }
    }
}
