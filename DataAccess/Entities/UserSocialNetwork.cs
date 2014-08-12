using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.DataAccess.Entities
{
    public class UserSocialNetwork:BaseEntity
    {
        public virtual User User { get; set; }
        public virtual SocialNetwork SocialNetwork { get; set; }
        public virtual string ThirdPartId { get; set; }
        public virtual string AccessToken { get; set; }
        public virtual string TokenExpires { get; set; }
        public virtual string Photo { get; set; }
        public virtual string UserName { get; set; }
    }
}
