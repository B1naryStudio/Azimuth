using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.Infrastructure.Interfaces;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azimuth.DataProviders.Concrete
{
    public class DeezerApi
    {
        private readonly IWebClient _webClient;
        private const string BaseUri = "https://api.deezer.com/";

        public DeezerApi(IWebClient webClient)
        {
            _webClient = webClient;
        }

        public async Task<DeezerTrackData> GetTrackInfo(string artist, string trackName)
        {
            var url = BaseUri + "search/" +
                      "autocomplete?q=" + Uri.EscapeDataString(artist) + " " + Uri.EscapeDataString(trackName);

            var json = JObject.Parse(await _webClient.GetWebData(url));
            var tracks = JsonConvert.DeserializeObject<List<DeezerTrackData>>(json["tracks"]["data"].ToString());
            return tracks.FirstOrDefault(track => (trackName.ToLower() == track.Title.ToLower()) && (artist.ToLower() == track.Artist.Name.ToLower()));
        }
    }
}