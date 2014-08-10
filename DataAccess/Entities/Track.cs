﻿using Azimuth.DataAccess.Infrastructure;
using Iesi.Collections.Generic;

namespace Azimuth.DataAccess.Entities
{
    public class Track : BaseEntity
    {
        public virtual string Lyrics { get; set; }

        public virtual string Duration { get; set; }

        // TODO: Create reference to Genre's Table
        public virtual string Genre { get; set; }

        public virtual Artist Artist { get; set; }

        public virtual Album Album { get; set; }

        public virtual ISet<Playlist> Playlists { get; set; }

        public Track()
        {
            Playlists = new HashedSet<Playlist>();
        }
    }
}