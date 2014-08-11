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
            var entity = GetOne(x => x.Identifier.User.Id == userId && x.Identifier.SocialNetwork.Id == socialNetworkId);
            _session.Delete(entity);
        }
    }
}
