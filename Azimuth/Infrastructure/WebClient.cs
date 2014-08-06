using System.Net.Http;
using System.Threading.Tasks;

namespace Azimuth.Infrastructure
{
    public class WebClient : IWebClient
    {
        public Task<string> GetWebData(string url)
        {
            var client = new HttpClient();
            return client.GetStringAsync(url);
        }
    }
}