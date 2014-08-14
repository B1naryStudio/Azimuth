
using System.Collections.Generic;
using Azimuth.DataAccess.Entities;

namespace Azimuth.Shared.Dto
{
    public class TracksDto
    {
        public string Name { get; set; }
        public string Duration { get; set; }
        public string Genre { get; set; }
        public string Url { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }



        //public virtual string Lyrics { get; set; }

        //public virtual string Duration { get; set; }

        //public virtual string Genre { get; set; }
        
        //public virtual string Url { get; set; }

        //public virtual string Name { get; set; }

        //public virtual Album Album { get; set; }

        //public virtual ICollection<Playlist> Playlists { get; set; }

        //public TracksDto()
        //{
        //    Playlists = new List<Playlist>();
        //}
    }
}
