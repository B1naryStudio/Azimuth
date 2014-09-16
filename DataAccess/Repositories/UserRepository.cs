using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;
using NHibernate.Linq;

namespace Azimuth.DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ISession session) : base(session)
        {
        }

        public User GetFullUserData(long id)
        {
            return _session.Query<User>()
                .FetchMany(x => x.Followers)
                .FetchMany(x => x.Following)
                .FetchMany(x => x.PlaylistFollowing)
                .FetchMany(x => x.SocialNetworks).AsEnumerable()
                .FirstOrDefault(x => x.Id == id);
        }

        public void Remove(User user)
        {
            _session.Delete(user);
        }
    }

    public interface IUserRepository : IRepository<User>
    {
        User GetFullUserData(long id);

        void Remove(User user);
    }
}
