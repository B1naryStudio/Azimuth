using System.Net.Http;
using System.Threading.Tasks;
using Azimuth.Infrastructure.Interfaces;
using TweetSharp;

namespace Azimuth.Infrastructure.Concrete
{
    public class WebClient : IWebClient
    {
        public Task<string> GetWebData(string url)
        {
            var client = new HttpClient();
            return client.GetStringAsync(url);
        }

        public Task<TwitterUser> GetWebData(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            var service = new TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(accessToken, accessTokenSecret);
            return Task.FromResult(service.GetUserProfile(new GetUserProfileOptions()));
        }
    }
}