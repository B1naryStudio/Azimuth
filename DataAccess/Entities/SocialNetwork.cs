
using Azimuth.DataAccess.Infrastructure;
using Iesi.Collections.Generic;

namespace Azimuth.DataAccess.Entities
{
    public class SocialNetwork:BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual ISet<UserSocialNetwork> Users { get; set; }

        public SocialNetwork()
        {
            Users = new HashedSet<UserSocialNetwork>();
        }
    }
}
