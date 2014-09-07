
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class VkTrackResponse
    {
        public class Audio
        {
            [JsonProperty(PropertyName = "aid")]
            public int AId { get; set; }
            [JsonProperty(PropertyName = "owner_id")]
            public int OwnerId { get; set; }
            [JsonProperty(PropertyName = "artist")]
            public string Artist { get; set; }
            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }
            [JsonProperty(PropertyName = "duration")]
            public int Duration { get; set; }
            [JsonProperty(PropertyName = "url")]
            public string Url { get; set; }
            [JsonProperty(PropertyName = "lyrics_id")]
            public string LyricsId { get; set; }
        }

            [JsonProperty(PropertyName = "response")]
            public List<Audio> Response { get; set; }
    }
}
