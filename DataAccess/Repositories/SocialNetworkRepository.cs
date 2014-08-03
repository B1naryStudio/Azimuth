using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;
using NHibernate.Linq;

namespace Azimuth.DataAccess.Repositories
{
    public class SocialNetworkRepository:BaseRepository<SocialNetwork>
    {
        private ISession _session;

        public SocialNetworkRepository(ISession session) : base(session)
        {
            _session = session;
        }

        public SocialNetwork GetSN(string name)
        {
            var sn = _session.Query<SocialNetwork>().Where(s => s.Name == name).ToList();
            return sn[0];
        }
    }
}
