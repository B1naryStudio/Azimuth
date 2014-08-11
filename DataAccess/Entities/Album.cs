using System.Collections.Generic;
using System.ComponentModel.Design;
using Azimuth.DataAccess.Infrastructure;
using Iesi.Collections;
using Iesi.Collections.Generic;

namespace Azimuth.DataAccess.Entities
{
    public class Album : BaseEntity
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual Artist Artist { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }

        public Album()
        {
            Tracks = new List<Track>();
        }
    }
}