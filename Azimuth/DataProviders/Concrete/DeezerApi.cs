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
            string artistUrl, artistJson, albumUrl, albumJson, trackJson, topTracksUrl, topTracks;
            DeezerTrackData.TrackData trackData;
            DeezerTrackData.Track resultSearch;

            List<DeezerTrackData.TrackData> topTrackList = new List<DeezerTrackData.TrackData>();

            trackJson = await _webClient.GetWebData(searchUrl);
            resultSearch = JsonConvert.DeserializeObject<DeezerTrackData.Track>(trackJson);
            trackData = resultSearch.Tracks.Datas.FirstOrDefault(author => author.Artist.Name.ToLower() == artist.ToLower());
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

                    topTracksUrl = BaseUri + "artist/" + trackData.Artist.Id + "/top?limit=50";
                    topTracks = await _webClient.GetWebData(topTracksUrl);

                    topTrackList = JsonConvert.DeserializeObject<List<DeezerTrackData.TrackData>>((JObject.Parse(topTracks))["data"].ToString());

                    foreach (var topItem in topTrackList)
                    {
                        if (trackData.TopTracks.Any(s => s.Title == topItem.Title))
                            continue;
                        trackData.TopTracks.Add(topItem);
                        if (trackData.TopTracks.Count >= 10)
                            break;
                    }
                }
                return trackData;
            }
            artistUrl = BaseUri + "artist/" + trackData.Artist.Id;
            artistJson = await _webClient.GetWebData(artistUrl);
            trackData.Artist = JsonConvert.DeserializeObject<DeezerTrackData.Artist>(artistJson);
            albumUrl = BaseUri + "album/" + trackData.Album.Id;
            albumJson = await _webClient.GetWebData(albumUrl);
            trackData.Album = JsonConvert.DeserializeObject<DeezerTrackData.Album>(albumJson);

            topTracksUrl = BaseUri + "artist/" + trackData.Artist.Id + "/top?limit=50";
            topTracks = await _webClient.GetWebData(topTracksUrl);

            topTrackList = JsonConvert.DeserializeObject<List<DeezerTrackData.TrackData>>((JObject.Parse(topTracks))["data"].ToString());

            foreach (var topItem in topTrackList)
            {
                if (trackData.TopTracks.Any(s => s.Title == topItem.Title))
                    continue;
                trackData.TopTracks.Add(topItem);
                if (trackData.TopTracks.Count >= 10)
                    break;
            }
            return trackData;
        }
    }
}