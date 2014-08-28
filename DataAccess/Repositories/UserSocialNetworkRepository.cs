using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;
using NHibernate.Linq;

namespace Azimuth.DataAccess.Repositories
{
    public class UserSocialNetworkRepository:BaseRepository<UserSocialNetwork>
    {
        public UserSocialNetworkRepository(ISession session): base(session)
        {
        }

        public UserSocialNetwork GetByThirdPartyId(string id)
        {
            return _session.Query<UserSocialNetwork>().FirstOrDefault(s => s.ThirdPartId == id);
        }

        public void Remove(long userId, long socialNetworkId)
        {
            var entity = GetOne(x => x.User.Id == userId && x.SocialNetwork.Id == socialNetworkId);
            _session.Delete(entity);
        }

        public void ChangeUserId(UserSocialNetwork userSn)
        {
            _session.Evict(userSn);
            _session.Save(userSn);
        }

    }
}
