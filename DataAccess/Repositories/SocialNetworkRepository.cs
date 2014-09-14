using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;
using NHibernate.Linq;

namespace Azimuth.DataAccess.Repositories
{
    public class SocialNetworkRepository : BaseRepository<SocialNetwork>, ISocialNetworkRepository
    {
        public SocialNetworkRepository(ISession session) : base(session)
        {
        }

        public SocialNetwork GetByName(string name)
        {
            return _session.Query<SocialNetwork>().FirstOrDefault(s => s.Name == name);
        }
    }

    public interface ISocialNetworkRepository : IRepository<SocialNetwork>
    {
        SocialNetwork GetByName(string name);
    }
}
