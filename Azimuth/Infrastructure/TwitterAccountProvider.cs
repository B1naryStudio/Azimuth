using System;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;

namespace Azimuth.Infrastructure
{
    public class TwitterAccountProvider: AccountProvider
    {
        private readonly string _accessToken;
        private readonly string _tokenExpiresIn;
        private readonly string _userId;
        private readonly string _accessTokenSecret;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        public TwitterAccountProvider(IWebClient webClient, string userId, string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret)
            :base(webClient)
        {
            this._accessToken = accessToken;
            this._userId = userId;
            this._accessTokenSecret = accessTokenSecret;
            this._consumerKey = consumerKey;
            this._consumerSecret = consumerSecret;
        }
        public override async Task<User> GetUserInfoAsync(string email = "")
        {
            var user = await _webClient.GetWebData(_consumerKey, _consumerSecret, _accessToken, _accessTokenSecret);

            return (User) user;
        }
    }
}