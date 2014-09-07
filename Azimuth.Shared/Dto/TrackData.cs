using System.Collections.Generic;
using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class TrackData
    {
        [JsonProperty(PropertyName = "response")]
        public VkAudioResponse Response { get; set; }

        public class Audio
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
            public int LyricsId { get; set; }
            [JsonProperty(PropertyName = "genre_id")]
            public Genres GenreId { get; set; }
        }

        public class VkAudioResponse
        {
            [JsonProperty(PropertyName = "count")]
            public int Count { get; set; }
            [JsonProperty(PropertyName = "items")]
            public List<Audio> Audios { get; set; }

            public VkAudioResponse()
            {
                Audios = new List<Audio>();
            }
        }

        public enum Genres //https://vk.com/dev/audio_genres
        {
            Undefined = 0,
            Rock,
            Pop,
            RapAndHipHop,
            EasyListening,
            DanceAndHouse,
            Instrumental,
            Metal,
            Dubstep,
            JazzAndBlues,
            DrumAndBass,
            Trance,
            Chanson,
            Ethnic,
            AcousticAndVocal,
            Reggae,
            Classical,
            IndiePop,
            Other,
            Speech,
            Alternative = 21,
            ElectropopAndDisco
        }
    }
}
