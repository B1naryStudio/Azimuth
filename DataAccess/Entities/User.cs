using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public virtual ICollection<UserSocialNetwork> SocialNetworks { get; set; }
        public virtual ICollection<User> Followers { get; set; }
        public virtual ICollection<User> Following { get; set; }
        public virtual ICollection<PlaylistLike> PlaylistFollowing  { get; set; }

        public User()
        {
            SocialNetworks = new List<UserSocialNetwork>();
            Followers = new List<User>();
            Following = new List<User>();
            PlaylistFollowing = new List<PlaylistLike>();
        }
        public override string ToString()
        {
            return Name.FirstName ??
                   String.Empty + Name.LastName ??
                   String.Empty + ScreenName ??
                   String.Empty + Gender ??
                   String.Empty + Email ??
                   String.Empty + Birthday ??
                   String.Empty + Timezone ??
                   String.Empty + ((Location != null) ? Location.City ?? String.Empty : String.Empty) +
                   ", " + ((Location != null) ? Location.Country ?? String.Empty : String.Empty) + Photo ?? String.Empty;
        }
    }
}
