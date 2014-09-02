using System;
using System.Threading.Tasks;
using System.Xml;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Infrastructure.Interfaces;

namespace Azimuth.DataProviders.Concrete
{
    public class ChartLyricsApi: IChartLyricsApi
    {
        private readonly IWebClient _webClient;
        private const string BaseUri = "http://api.chartlyrics.com/apiv1.asmx/";

        public ChartLyricsApi(IWebClient webClient)
        {
            _webClient = webClient;
        }

        public async Task<string> GetTrackLyric(string author, string trackName)
        {
            string url = BaseUri + "SearchLyricDirect?artist=" + author + "&song=" + trackName;
            string trackLyricXml = await _webClient.GetWebData(url);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(trackLyricXml);
            string trackLyric = "";

            try
            {
                trackLyric = xmlDoc.GetElementsByTagName("Lyric").Item(0).InnerText;
                trackLyric = trackLyric.Replace("\r\n", "<br>");
                return trackLyric;
            }
            catch (NullReferenceException exception)
            {
                throw new BadParametersException("Wrong track or artist name", 404);
            }
        }
    }
}