using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure.Interfaces;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azimuth.DataProviders.Concrete
{
    public class DeezerApi : IMusicService<DeezerTrackData.TrackData>
    {
        private readonly IWebClient _webClient;
        private const string BaseUri = "https://api.deezer.com/";

        public DeezerApi(IWebClient webClient)
        {
            _webClient = webClient;
        }

        public async Task<DeezerTrackData.TrackData> GetTrackInfo(string artist, string trackName)
        {
            var searchUrl = BaseUri + "search/" +
                      "autocomplete?q=" + Uri.EscapeDataString(artist) + " " + Uri.EscapeDataString(trackName);

            var trackJson = await _webClient.GetWebData(searchUrl);
            var resultSearch = JsonConvert.DeserializeObject<DeezerTrackData.Track>(trackJson);
            var trackData = resultSearch.Tracks.Datas.FirstOrDefault();
            if (trackData == null)
                return null;
            var artistUrl = BaseUri + "artist/" + trackData.Artist.Id;
            var artistJson = await _webClient.GetWebData(artistUrl);
            trackData.Artist = JsonConvert.DeserializeObject<DeezerTrackData.Artist>(artistJson);
            var albumUrl = BaseUri + "album/" + trackData.Album.Id;
            var albumJson = await _webClient.GetWebData(albumUrl);
            trackData.Album = JsonConvert.DeserializeObject<DeezerTrackData.Album>(albumJson);

            return trackData;
        }
    }
}