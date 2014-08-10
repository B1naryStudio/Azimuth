﻿using Azimuth.DataAccess.Infrastructure;
using Iesi.Collections.Generic;

namespace Azimuth.DataAccess.Entities
{
    public class Artist : BaseEntity
    {
        public virtual Name Name { get; set; }

        public virtual string Site { get; set; }

        public virtual string Description { get; set; }

        public virtual ISet<Album> Albums { get; set; }

        public virtual ISet<Track> Tracks { get; set; }
    }
}