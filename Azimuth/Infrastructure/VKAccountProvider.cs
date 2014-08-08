using System;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;

namespace Azimuth.Infrastructure
{
    public class VKAccountProvider : AccountProvider
    {
        private readonly string _userId;
        private readonly string _accessToken;
        public string UserInfoUrl { get; private set; }

        public VKAccountProvider(IWebClient webClient, UserCredential userCredential)
            :base(webClient)
        {
            _userId = userCredential.SocialNetworkId;
            _accessToken = userCredential.AccessToken;

            UserInfoUrl = String.Format(
                @"https://api.vk.com/method/users.get?user_id={0}&fields=screen_name,bdate,sex,city,country,photo_max_orig,timezone&v=5.23&access_token={1}",
                _userId,
                _accessToken);
        }

        public override async Task<User> GetUserInfoAsync(string email = "")
        {
            var response = await _webClient.GetWebData(UserInfoUrl);
            var userData = JsonConvert.DeserializeObject<VKUserdata.Response>(response);

            var city = userData.response.First().city.title;
            var country = userData.response.First().country.title;

            User currentUser = (User) userData;
            currentUser.Location = new Location
            {
                City = city ?? String.Empty,
                Country = country ?? String.Empty
            };
            currentUser.Email = email ?? String.Empty;

            return currentUser;
        }
    }
}