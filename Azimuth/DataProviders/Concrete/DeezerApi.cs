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
            string searchUrl = BaseUri + "search/" +
                      "autocomplete?q=" + Uri.EscapeDataString(artist) + " " + Uri.EscapeDataString(trackName);
            string artistUrl, artistJson, albumUrl, albumJson, trackJson;
            DeezerTrackData.TrackData trackData;
            DeezerTrackData.Track resultSearch;

            trackJson = await _webClient.GetWebData(searchUrl);
            resultSearch = JsonConvert.DeserializeObject<DeezerTrackData.Track>(trackJson);
            trackData = resultSearch.Tracks.Datas.FirstOrDefault();
            if (trackData == null)
            {
                searchUrl = BaseUri + "search/" + "autocomplete?q=" + Uri.EscapeDataString(artist);
                trackJson = await _webClient.GetWebData(searchUrl);
                resultSearch = JsonConvert.DeserializeObject<DeezerTrackData.Track>(trackJson);
                trackData = resultSearch.Tracks.Datas.FirstOrDefault();
                if (trackData != null)
                {
                    artistUrl = BaseUri + "artist/" + trackData.Artist.Id;
                    artistJson = await _webClient.GetWebData(artistUrl);
                    trackData.Artist = JsonConvert.DeserializeObject<DeezerTrackData.Artist>(artistJson);
                    trackData.Album = null;
                }
                return trackData;
            }
            artistUrl = BaseUri + "artist/" + trackData.Artist.Id;
            artistJson = await _webClient.GetWebData(artistUrl);
            trackData.Artist = JsonConvert.DeserializeObject<DeezerTrackData.Artist>(artistJson);
            albumUrl = BaseUri + "album/" + trackData.Album.Id;
            albumJson = await _webClient.GetWebData(albumUrl);
            trackData.Album = JsonConvert.DeserializeObject<DeezerTrackData.Album>(albumJson);

            return trackData;
        }
    }
}