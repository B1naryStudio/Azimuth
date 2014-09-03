using System.Collections.Generic;
using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{

    public class DeezerTrackData
    {
        public class Artist
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
            [JsonProperty(PropertyName = "link")]
            public string Link { get; set; }
            [JsonProperty(PropertyName = "picture")]
            public string Picture { get; set; }
            [JsonProperty(PropertyName = "nb_album")]
            public int AlbumsNumber { get; set; }
            [JsonProperty(PropertyName = "nb_fan")]
            public int FansNumber { get; set; }
            [JsonProperty(PropertyName = "radio")]
            public bool Radio { get; set; }
            [JsonProperty(PropertyName = "tracklist")]
            public string TrackList { get; set; }
            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }

        }
            public class Datum
            {
                [JsonProperty(PropertyName = "id")]
                public int Id { get; set; }
                [JsonProperty(PropertyName = "name")]
                public string Name { get; set; }
                [JsonProperty(PropertyName = "type")]
                public string Type { get; set; }
            }

            public class Genres
            {
                [JsonProperty(PropertyName = "data")]
                public List<Datum> Data { get; set; }
            }

            public class Album
            {
                [JsonProperty(PropertyName = "id")]
                public string Id { get; set; }
                [JsonProperty(PropertyName = "title")]
                public string Title { get; set; }
                [JsonProperty(PropertyName = "upc")]
                public string Upc { get; set; }
                [JsonProperty(PropertyName = "link")]
                public string link { get; set; }
                [JsonProperty(PropertyName = "cover")]
                public string Cover { get; set; }
                [JsonProperty(PropertyName = "genre_id")]
                public int GenreId { get; set; }
                [JsonProperty(PropertyName = "genres")]
                public Genres Genres { get; set; }
                [JsonProperty(PropertyName = "label")]
                public string Label { get; set; }
                [JsonProperty(PropertyName = "nb_tracks")]
                public int NumberOfTracks { get; set; }
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
                [JsonProperty(PropertyName = "available")]
                public bool Available { get; set; }
                [JsonProperty(PropertyName = "tracklist")]
                public string TrackList { get; set; }
            }

        public class TrackData
        {
            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }
            [JsonProperty(PropertyName = "readable")]
            public bool Readable { get; set; }
            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }
            [JsonProperty(PropertyName = "link")]
            public string Link { get; set; }
            [JsonProperty(PropertyName = "duration")]
            public int Duration { get; set; }
            [JsonProperty(PropertyName = "rank")]
            public int Rank { get; set; }
            [JsonProperty(PropertyName = "preview")]
            public string Preview { get; set; }
            [JsonProperty(PropertyName = "artist")]
            public Artist Artist { get; set; }
            [JsonProperty(PropertyName = "album")]
            public Album Album { get; set; }
            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }
            [JsonProperty(PropertyName = "topTracks")]
            public List<TrackData> TopTracks { get; set; }
        }

        public class Data
        {
            [JsonProperty(PropertyName = "data")]
            public List<TrackData> Datas { get; set; }
        }

        public class Track
        {
            [JsonProperty(PropertyName = "tracks")]
            public Data Tracks { get; set; }
        }
    }
}
