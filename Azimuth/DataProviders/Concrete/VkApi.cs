using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using Azimuth.Infrastructure.Exceptions;
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
            while ((i == 0) || (count > (i*MaxCntTracksPerReq)))
            {
                var url = BaseUri + "audio.get" +
                            "?owner_id=" + Uri.EscapeDataString(userId) +
                            "&need_user=0" + //without user_info
                            "&v=5.24" +
                            "&count=" + MaxCntTracksPerReq + //max count tracks for 1 req
                            "&offset=" + (MaxCntTracksPerReq*i) +
                            "&access_token=" + Uri.EscapeDataString(accessToken);

                var json = JObject.Parse(await _webClient.GetWebData(url));

                var response = json["response"];

                if (response != null)
                {
                    count = response["count"].Value<int>();
                    tracks.AddRange(
                        JsonConvert.DeserializeObject<List<TrackData>>(
                            JArray.Parse(json["response"]["items"].ToString()).ToString()));

                    i++;
                }
                else
                {
                    var error = json["error"];
                    if (error == null) 
                        return tracks;
                    var code = error["error_code"].Value<int>();
                    var message = error["error_msg"].ToString();
                    switch (code)
                    {
                        case 1:
                            throw new UnknownErrorException(message, code);
                        case 2:
                            throw new ApplicationDisabledException(message, code);
                        case 4:
                            throw new IncorrectSignatureException(message, code);
                        case 5:
                            throw new UserAuthorizationException(message, code);
                        case 6:
                            throw new ManyRequestException(message, code);
                        case 100:
                            throw new BadParametersException(message, code);
                        default:
                            throw new VkApiException(message, code);
                    }
                }
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
            return JsonConvert.DeserializeObject<TrackData>(JArray.Parse(json["response"].ToString()).ToString());
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
            return JArray.Parse(json["response"]["text"].ToString()).ToString();
        }
    }
}