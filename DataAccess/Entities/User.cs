using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.DataAccess.Entities
{
    public class User : BaseEntity
    {
        public virtual Name Name { get; set; }
        public virtual string ScreenName { get; set; }
        public virtual string Gender { get; set; }
        public virtual string Birthday { get; set; }
        public virtual string Photo { get; set; }
        public virtual int Timezone { get; set; }
        public virtual Location Location { get; set; }
        public virtual string Email { get; set; }
        public virtual Iesi.Collections.Generic.ISet<UserSocialNetwork> SocialNetworks { get; set; }

        public override string ToString()
        {
            return Name.FirstName + Name.LastName + ScreenName + Gender + Email + Birthday + Timezone + Location.City +
                   ", " + Location.Country; // Think about location format
        }
    }
}
