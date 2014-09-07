
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class TrackSocialInfo
    {
        [JsonProperty(PropertyName = "trackData")]
        public List<Track> Tracks { get; set; }

        public class Track
        {
            [JsonProperty(PropertyName = "ownerId")]
            public string OwnerId { get; set; }
            [JsonProperty(PropertyName = "thirdPartId")]
            public string ThirdPartId { get; set; }    
        }
    }
}
