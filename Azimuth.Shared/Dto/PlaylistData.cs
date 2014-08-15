using System.Collections.Generic;

namespace Azimuth.Shared.Dto
{
    public class PlaylistData
    {
        public long Id { get; set; }
        public string Name;
        public List<TracksDto> Tracks;
    }
}
