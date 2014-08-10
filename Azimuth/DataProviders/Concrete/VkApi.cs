using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataProviders.Interfaces;
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
                var userTracksUrl = BaseUri + "audio.get" +
                                    "?owner_id=" + Uri.EscapeDataString(userId) +
                                    "&need_user=0" + //without user_info
                                    "&v=5.24" +
                                    "&count=" + MaxCntTracksPerReq + //max count tracks for 1 req
                                    "&offset=" + (MaxCntTracksPerReq*i) +
                                    "&access_token=" + Uri.EscapeDataString(accessToken);

                var json = JObject.Parse(await FetchData(userTracksUrl));
                count = json["response"]["count"].Value<int>();
                tracks.AddRange(JsonConvert.DeserializeObject<List<VKTrackData>>(CorrectData(json)));

                i++;
            } 

            return tracks;
        }

        private async Task<string> FetchData(string uri)
        {
            //var client = new System.Net.WebClient { Encoding = Encoding.UTF8 };
            //var json = client.DownloadString(uri);
            var json = await _webClient.GetWebData(uri);
            return json;
        }

        private string CorrectData(JObject json)
        {
            return JArray.Parse(json["response"]["items"].ToString()).ToString();
        }
    }
}