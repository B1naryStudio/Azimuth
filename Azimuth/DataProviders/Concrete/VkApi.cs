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
    public class VkApi : IVkApi
    {
        private readonly IWebClient _webClient;
        private const string BaseUri = "https://api.vk.com/method/";
        private const int MaxCntTracksPerReq = 6000;

        public VkApi(IWebClient webClient)
        {
            _webClient = webClient;
        }

        public async Task<List<VKTrackData>> GetUserTracks(string userId, string accessToken)
        {
            var tracks = new List<VKTrackData>();
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
                tracks.AddRange(JsonConvert.DeserializeObject<List<VKTrackData>>(JArray.Parse(json["response"]["items"].ToString()).ToString()));

                i++;
            } 

            return tracks;
        }

        public async Task<VKTrackData> GetTrackById(string userId, string trackId, string accessToken)
        {
            var url = BaseUri + "audio.getById" +
                      "?audios=" + Uri.EscapeDataString(userId + "_" + trackId) +
                      "&v=5.24" +
                      "&access_token=" + Uri.EscapeDataString(accessToken);

            var json = JObject.Parse(await _webClient.GetWebData(url));
            return JsonConvert.DeserializeObject<VKTrackData>(JArray.Parse(json["response"].ToString()).ToString());
        }

        public async Task<List<VKTrackData>> GetTracksById(string userId, List<string> trackIds, string accessToken)
        {
            var tracks = new List<VKTrackData>();
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
            return JArray.Parse(json["response"]["text"].ToString()).ToString();
        }
    }
}