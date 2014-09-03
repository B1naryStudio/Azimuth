using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class DeezerTrackData
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "link")]
        public string Link { get; set; }
        [JsonProperty(PropertyName = "rank")]
        public int Rank { get; set; }
        [JsonProperty(PropertyName = "artist")]
        public DeezerArtistData Artist { get; set; }
        [JsonProperty(PropertyName = "album")]
        public DeezerAlbumData Album { get; set; }
    }
}
