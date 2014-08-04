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
        public virtual string ThirdPartId { get; set; }
        public virtual string AccessToken { get; set; }
        public virtual string TokenExpires { get; set; }
    }

    public class UserSNIdentifier
    {
        public virtual User User { get; set; }
        public virtual SocialNetwork SocialNetwork { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            UserSNIdentifier id;
            id = (UserSNIdentifier)obj;
            if (User == id.User && SocialNetwork == id.SocialNetwork)
            {
                return true;
            }
            return false; ;
        }

        public override int GetHashCode()
        {
            return (User + "|" + SocialNetwork).GetHashCode();
        }
    }
}
