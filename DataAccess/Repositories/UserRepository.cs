using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;

namespace Azimuth.DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(ISession session) : base(session)
        {
        }
    }
}
