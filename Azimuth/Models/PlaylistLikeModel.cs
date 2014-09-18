using System.Collections.Generic;

namespace Azimuth.Models
{
    public class PlaylistLikeModel
    {
        public string Name { get; set; }

        public long Id { get; set; }

        public bool IsFavorite { get; set; }

        public bool IsLiked { get; set; }

        public long Duration { get; set; }

        public long Songs { get; set; }
        public List<string> Genres { get; set; }
    }
}