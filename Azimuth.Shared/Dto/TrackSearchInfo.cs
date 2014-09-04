
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class TrackSearchInfo
    {
        [JsonProperty(PropertyName = "trackdata")]
        public List<SearchData> TrackDatas  { get; set; }

        public class SearchData
        {
            [JsonProperty(PropertyName = "artist")]
            public string Artist { get; set; }
            [JsonProperty(PropertyName = "track")]
            public string Name { get; set; }
        }
    }
}
