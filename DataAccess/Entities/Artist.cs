using System.Collections.Generic;
using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.DataAccess.Entities
{
    public class Artist : BaseEntity
    {
        public virtual string Name { get; set; }

        public virtual string Site { get; set; }

        public virtual string Description { get; set; }

        public virtual ICollection<Album> Albums { get; set; }

        public Artist()
        {
            Albums = new List<Album>();
        }
    }
}