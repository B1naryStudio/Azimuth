using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class DeezerAlbumData
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "cover")]
        public string Cover { get; set; }
        [JsonProperty(PropertyName = "genres")]
        public string Genres { get; set; }
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }
        [JsonProperty(PropertyName = "fans")]
        public int Fans { get; set; }
        [JsonProperty(PropertyName = "rating")]
        public int Rating { get; set; }
        [JsonProperty(PropertyName = "release_date")]
        public string ReleaseDate { get; set; }
        [JsonProperty(PropertyName = "record_type")]
        public string RecordType { get; set; }
    }
}
