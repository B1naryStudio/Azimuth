using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TweetSharp;

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

        public string UserInfoUrl { get; set; }

        public TwitterAccountProvider(string userId, string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret)
        {
            this._accessToken = accessToken;
            this._userId = userId;
            this._accessTokenSecret = accessTokenSecret;
            this._consumerKey = consumerKey;
            this._consumerSecret = consumerSecret;

            //UserInfoUrl = String.Format(@"https://api.twitter.com/1.1/users/show.json?user_id={0}", _userId, accessToken);

        }
        public override async Task<User> GetUserInfoAsync(string email = "")
        {
            //var service = new TwitterService("WUOz1dJWadM5NSUmgMrcPgiIa", "9tO77dgpGcQuve4MDf0ZTKuHY3TVw8QLpjRTCTxDXh9vJpQXyc");
            var service = new TwitterService(_consumerKey, _consumerSecret);
            service.AuthenticateWith(_accessToken, _accessTokenSecret);
            var user = service.GetUserProfile(new GetUserProfileOptions());

            return new User()
            {
                Name = new Name() { FirstName = user.Name, LastName = String.Empty},
                Birthday = String.Empty, //
                Email = String.Empty, //
                Gender = String.Empty, //
                Location = new DataAccess.Entities.Location() { City = user.Location, Country = String.Empty},
                Timezone = -100,
                ScreenName = user.ScreenName,
                Photo = user.ProfileImageUrl
            };
        }
    }
}