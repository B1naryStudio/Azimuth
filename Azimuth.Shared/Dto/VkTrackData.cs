
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class VkTrackData
    {
        [JsonProperty(PropertyName = "response")]
        public Response ResponseData { get; set; }

        public class Track
        {
            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }
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
            public long LyricsId { get; set; }
            [JsonProperty(PropertyName = "genre_id")]
            public int GenreId { get; set; }
            [JsonProperty(PropertyName = "album_id")]
            public int? AlbumId { get; set; }
        }

        public class Response
        {
            [JsonProperty(PropertyName = "count")]
            public int Count { get; set; }
            [JsonProperty(PropertyName = "items")]
            public List<Track> Tracks { get; set; }
        }
    }
}
