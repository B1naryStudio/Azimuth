using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;
using NHibernate.Linq;

namespace Azimuth.DataAccess.Repositories
{
    public class SocialNetworkRepository:BaseRepository<SocialNetwork>
    {
        public SocialNetworkRepository(ISession session) : base(session)
        {
        }

        public SocialNetwork GetByName(string name)
        {
            return _session.Query<SocialNetwork>().FirstOrDefault(s => s.Name == name);
        }
    }
}
