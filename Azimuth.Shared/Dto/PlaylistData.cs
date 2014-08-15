using System.Collections.Generic;
using Azimuth.DataAccess.Entities;

namespace Azimuth.Shared.Dto
{
    public class PlaylistData
    {
        public long Id { get; set; }
        public string Name;
        public Accessibilty Accessibilty;
        public List<string> TrackIds;
        public List<TracksDto> Tracks;
    }
}
