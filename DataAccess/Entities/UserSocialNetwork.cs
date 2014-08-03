using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azimuth.DataAccess.Infrastructure;
using Castle.Components.DictionaryAdapter;

namespace Azimuth.DataAccess.Entities
{
    public class UserSocialNetwork:BaseEntity
    {
        public virtual UserSNIdentifier Identifier { get; set; }
        public virtual User User { get; set; }
        public virtual SocialNetwork SocialNetwork { get; set; }
        public virtual string ThirdPartId { get; set; }
    }

    public class UserSNIdentifier
    {
        public virtual long UserId { get; set; }
        public virtual long SocialNetworkId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            UserSNIdentifier id;
            id = (UserSNIdentifier) obj;
            if (UserId == id.UserId && SocialNetworkId == id.SocialNetworkId)
            {
                return true;
            }
            return false;;
        }

        public override int GetHashCode()
        {
            return (UserId + "|" + SocialNetworkId).GetHashCode();
        }
    }
}
