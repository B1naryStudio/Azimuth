using Azimuth.DataAccess.Infrastructure;
using Iesi.Collections.Generic;
using TweetSharp;

namespace Azimuth.DataAccess.Entities
{
    public class Playlist : BaseEntity
    {
        public virtual string Name { get; set; }

        public virtual string Accessibilty { get; set; }

        public virtual User Creator { get; set; }

        public virtual ISet<Track> Tracks { get; set; }
    }
}