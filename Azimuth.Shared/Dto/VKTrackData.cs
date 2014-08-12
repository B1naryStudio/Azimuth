using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class VKTrackData
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "artist")]
        public string Artist { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }

        [JsonProperty(PropertyName = "lyrics_id")]
        public long LyricsId { get; set; }

        [JsonProperty(PropertyName = "genre_id")]
        public int GenreId { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; } 
    }
}
