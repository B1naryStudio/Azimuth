
using System.Collections.Generic;

namespace Azimuth.Shared.Dto
{
    public class PublicPlaylistTracksViewModel
    {
        public List<TracksDto> Tracks { get; set; }
        public PublicPlaylistInfo CurrentPlaylist { get; set; }
    }
}
