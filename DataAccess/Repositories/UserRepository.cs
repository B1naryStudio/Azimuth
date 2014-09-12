using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;
using NHibernate.Linq;

namespace Azimuth.DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(ISession session) : base(session)
        {
        }

        public User GetFullUserData(long id)
        {
            return _session.Query<User>()
                .Fetch(x => x.Followers)
                .Fetch(x => x.Following)
                .Fetch(x => x.PlaylistFollowing)
                .Fetch(x => x.SocialNetworks)
                .FirstOrDefault(x => x.Id == id);
        }

        public void Remove(User user)
        {
            _session.Delete(user);
        }
    }
}
