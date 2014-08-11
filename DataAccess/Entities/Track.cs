﻿using System.Collections.Generic;
using Azimuth.DataAccess.Infrastructure;
using Iesi.Collections.Generic;

namespace Azimuth.DataAccess.Entities
{
    public class Track : BaseEntity
    {
        public virtual string Lyrics { get; set; }

        public virtual string Duration { get; set; }

        public virtual string Genre { get; set; }
        
        public virtual string Url { get; set; }

        public virtual string Name { get; set; }

        public virtual Album Album { get; set; }

        public virtual ICollection<Playlist> Playlists { get; set; }

        public Track()
        {
            Playlists = new List<Playlist>();
        }
    }
}