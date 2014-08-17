using System.Collections.Generic;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.Shared.Enums;
using NHibernate.Type;

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

    public class AccessibiltyType : EnumStringType<Accessibilty>
    {
    }
}