using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Azimuth.Shared.Dto
{
    public class TrackInfoDto
    {
        [JsonProperty(PropertyName = "track")]
        public TrackResponse Track { get; set; }
    }

    public class TrackResponse
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "artist")]
        public TrackArtist TrackArtist { get; set; }

        [JsonProperty(PropertyName = "album")]
        public TrackAlbum TrackAlbum { get; set; }

        [JsonProperty(PropertyName = "wiki")]
        public TrackWiki TrackWiki { get; set; }
    }

    public class TrackWiki
    {
        [JsonProperty(PropertyName = "published")]
        public string Published { get; set; }

        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
    }

    public class TrackAlbum
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "image")]
        public List<AlbumImage> AlbumImages { get; set; }
    }

    public class AlbumImage
    {
        [JsonProperty(PropertyName = "size")]
        public string Size { get; set; }

        [JsonProperty(PropertyName = "#text")]
        public string Url { get; set; }
    }

    public class TrackArtist
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}