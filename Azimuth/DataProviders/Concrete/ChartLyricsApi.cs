using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Infrastructure.Interfaces;

namespace Azimuth.DataProviders.Concrete
{
    public class ChartLyricsApi: IMusicService<string[]>
    {
        private readonly IWebClient _webClient;
        private const string BaseUri = "http://api.chartlyrics.com/apiv1.asmx/";

        public ChartLyricsApi(IWebClient webClient)
        {
            _webClient = webClient;
        }

        public async Task<string[]> GetTrackInfo(string author, string trackName)
        {
            string url = BaseUri + "SearchLyricDirect?artist=" + author + "&song=" + trackName;
            string trackLyricXml;
            try
            {
                trackLyricXml = await _webClient.GetWebData(url);
            }
            catch (Exception)
            {
                return null;
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(trackLyricXml);
            string trackLyric = "";

            try
            {
                trackLyric = xmlDoc.GetElementsByTagName("Lyric").Item(0).InnerText;
                string[] separators = new string[] {"\r\n"};
                var splittedLyric = trackLyric.Split(separators, StringSplitOptions.None);
                return splittedLyric;
            }
            catch (NullReferenceException)
            {
                throw new BadParametersException("Wrong track or artist name", 404);
            }
        }
    }
}