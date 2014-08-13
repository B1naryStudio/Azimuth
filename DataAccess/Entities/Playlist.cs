using System.Collections.Generic;
using Azimuth.DataAccess.Infrastructure;
using Iesi.Collections.Generic;
using NHibernate.Type;
using TweetSharp;

namespace Azimuth.DataAccess.Entities
{
    public class Playlist : BaseEntity
    {
        public virtual string Name { get; set; }

        public virtual Accessibilty Accessibilty { get; set; }

        public virtual User Creator { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }

        public Playlist()
        {
            Tracks = new List<Track>();
        }
    }

    public enum Accessibilty
    {
        Private,
        Public
    }

    public class AccessibiltyType : EnumStringType<Accessibilty>
    {
    }
}