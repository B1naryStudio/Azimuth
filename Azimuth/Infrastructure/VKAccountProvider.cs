using System;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;
using Location = Azimuth.DataAccess.Entities.Location;

namespace Azimuth.Infrastructure
{
    public class VKAccountProvider : AccountProvider
    {
        private readonly string _userId;
        private readonly string _accessToken;
        public string UserInfoUrl { get; private set; }

        public VKAccountProvider(IWebClient webClient, string userId, string accessToken)
            :base(webClient)
        {
            _userId = userId;
            _accessToken = accessToken;

            UserInfoUrl = String.Format(
                @"https://api.vk.com/method/users.get?user_id={0}&fields=screen_name,bdate,sex,city,country,photo_max_orig,timezone&v=5.23&access_token={1}",
                userId,
                accessToken);
        }

        public override async Task<User> GetUserInfoAsync(string email = "")
        {
            var response = await _webClient.GetWebData(UserInfoUrl);
            var userData = JsonConvert.DeserializeObject<VKUserdata.Response>(response);

            var city = userData.response[0].city.title;
            var country = userData.response[0].country.title;

            return new User
            {
                Name = new Name { FirstName = userData.response[0].first_name ?? String.Empty, LastName = userData.response[0].last_name ?? String.Empty },
                ScreenName = userData.response[0].screen_name ?? String.Empty,
                Gender = (userData.response[0].sex != 0) ? userData.response[0].sex.ToString() : String.Empty,
                Birthday = userData.response[0].bdate ?? String.Empty,
                Email = email ?? String.Empty,
                Location =
                    new Location
                    {
                        City = city ?? String.Empty,
                        Country = country ?? String.Empty
                    },
                Timezone = userData.response[0].timezone,
                Photo = userData.response[0].photo_max_orig
            };  
        }
    }
}