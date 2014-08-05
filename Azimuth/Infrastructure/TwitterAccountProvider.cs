using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azimuth.Infrastructure
{
    public class TwitterAccountProvider: AccountProvider
    {
        private readonly string _accessToken;
        private readonly string _tokenExpiresIn;
        private readonly string _userId;

        public string UserInfoUrl { get; set; }

        public TwitterAccountProvider(string userId, string accessToken)
        {
            this._accessToken = accessToken;
            this._userId = userId;

            UserInfoUrl = String.Format(@"",userId, accessToken);
        }
        public override async Task<User> GetUserInfoAsync(string email = "")
        {
            var userDataJson = await GetRequest(UserInfoUrl);
            var userDataObject = JObject.Parse(userDataJson);
            var userData = JsonConvert.DeserializeObject<TwitterUserData>(userDataJson);

            return new User();
        }
    }
}