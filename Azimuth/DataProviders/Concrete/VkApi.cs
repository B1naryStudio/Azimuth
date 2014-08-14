using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azimuth.DataProviders.Concrete
{
    public class VkApi : ISocialNetworkApi
    {
        private readonly IWebClient _webClient;
        private const string BaseUri = "https://api.vk.com/method/";
        private const int MaxCntTracksPerReq = 6000;

        public VkApi(IWebClient webClient)
        {
            _webClient = webClient;
        }

        public async Task<List<TrackData>> GetTracks(string userId, string accessToken)
        {
            var tracks = new List<TrackData>();
            int i = 0;
            int count = 0;

            //if user have more than 6000 tracks we must execute several req
            while ((i == 0) || (count > (i * MaxCntTracksPerReq)))
            {
                var url = BaseUri + "audio.get" +
                          "?owner_id=" + Uri.EscapeDataString(userId) +
                          "&need_user=0" + //without user_info
                          "&v=5.24" +
                          "&count=" + MaxCntTracksPerReq + //max count tracks for 1 req
                          "&offset=" + (MaxCntTracksPerReq*i) +
                          "&access_token=" + Uri.EscapeDataString(accessToken);

                var json = JObject.Parse(await _webClient.GetWebData(url));
                count = json["response"]["count"].Value<int>();
                tracks.AddRange(JsonConvert.DeserializeObject<List<TrackData>>(JArray.Parse(json["response"]["items"].ToString()).ToString()));

                foreach (var track in tracks)
                {
                    track.Genre = GetGenreById(Convert.ToInt32(track.Genre));
                }

                i++;
            } 

            return tracks;
        }

        public async Task<TrackData> GetTrackById(string userId, string trackId, string accessToken)
        {
            var url = BaseUri + "audio.getById" +
                      "?audios=" + Uri.EscapeDataString(userId + "_" + trackId) +
                      "&v=5.24" +
                      "&access_token=" + Uri.EscapeDataString(accessToken);

            var json = JObject.Parse(await _webClient.GetWebData(url));
            var track = JsonConvert.DeserializeObject<TrackData>(JArray.Parse(json["response"].ToString()).First.ToString());
            track.Genre = GetGenreById(Convert.ToInt32(track.Genre));
            return track;
        }

        public async Task<List<TrackData>> GetTracksById(string userId, List<string> trackIds, string accessToken)
        {
            var tracks = new List<TrackData>();
            foreach (var trackId in trackIds)
            {
                tracks.Add(await GetTrackById(userId, trackId, accessToken));
            }
            return tracks;
        }

        public async Task<string> GetLyricsById(string userId, long lyricsId, string accessToken)
        {
            var url = BaseUri + "audio.getLyrics" +
                      "?lyrics_id=" + Uri.EscapeDataString(lyricsId.ToString()) +
                      "&v=5.24" +
                      "&access_token=" + Uri.EscapeDataString(accessToken);

            var json = JObject.Parse(await _webClient.GetWebData(url));
            return json["response"]["text"].ToString();
        }

        private string GetGenreById(int id)
        {
            return Enum.GetName(typeof(Genres), id);
        }

        private enum Genres //https://vk.com/dev/audio_genres
        {
            Rock = 1,
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