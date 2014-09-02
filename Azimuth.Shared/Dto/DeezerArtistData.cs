using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class DeezerArtistData
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "link")]
        public string Link { get; set; }
        [JsonProperty(PropertyName = "picture")]
        public string Picture { get; set; }
        [JsonProperty(PropertyName = "nb_album")]
        public int AlbumCnt { get; set; }
        [JsonProperty(PropertyName = "nb_fan")]
        public int FanCnt { get; set; }
        [JsonProperty(PropertyName = "tracklist")]
        public string TopTracklist { get; set; }
    }
}
