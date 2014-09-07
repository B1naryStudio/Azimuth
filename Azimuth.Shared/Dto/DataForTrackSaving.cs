
using System.Collections.Generic;

namespace Azimuth.Shared.Dto
{
    public class DataForTrackSaving
    {
        public long PlaylistId { get; set; }
        public List<TrackInfo> TrackInfos { get; set; }

        public class TrackInfo
        {
            public string ThirdPartId { get; set; }
            public string OwnerId { get; set; }
        }
    }
}
