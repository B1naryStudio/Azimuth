
using System.Collections.Generic;
using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.DataAccess.Entities
{
    public class SocialNetwork:BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual ICollection<UserSocialNetwork> Users { get; set; }
        public SocialNetwork()
        {
            Users = new List<UserSocialNetwork>();
        }
    }
}
