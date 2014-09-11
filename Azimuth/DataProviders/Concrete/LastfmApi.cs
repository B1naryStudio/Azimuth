using System.Threading.Tasks;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure.Interfaces;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;

namespace Azimuth.DataProviders.Concrete
{
    public class LastfmApi : IMusicService<LastfmTrackData>
    {
        private readonly IWebClient _webClient;
        private const string BaseUri = "http://ws.audioscrobbler.com/2.0/?method=";
        private const string AppKey = "2006e803c24d07ed0403f9dcf96adc36";

        public LastfmApi(IWebClient webClient)
        {
            _webClient = webClient;
        }

        public async Task<LastfmTrackData> GetTrackInfo(string author, string trackName)
        {
            var url = BaseUri + "track.getInfo" +
                     "&api_key=" + AppKey +
                     "&artist=" + author +
                     "&track=" + trackName + 
                     "&format=json";

            var json = await _webClient.GetWebData(url);
            var trackInfo = JsonConvert.DeserializeObject<LastfmTrackData>(json);

            return trackInfo;
        }
    }
}