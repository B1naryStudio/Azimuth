using System.Collections.Generic;
using Azimuth.Shared.Enums;

namespace Azimuth.Shared.Dto
{
    public class PlaylistData
    {
        public long Id { get; set; }
        public string Name;
        public Accessibilty Accessibilty;
        public List<string> TrackIds;
        public List<string> Genres;
        public UserBrief Creator { get; set; }
        public int Duration;
        public int ItemsCount;
    }
}
