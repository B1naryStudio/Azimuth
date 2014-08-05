using System;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Location = Azimuth.DataAccess.Entities.Location;

namespace Azimuth.Infrastructure
{
    public class VKAccountProvider : AccountProvider
    {
        private readonly string _userId;
        private readonly string _accessToken;
        public string UserInfoUrl { get; private set; }

        public VKAccountProvider(string userId, string accessToken = "")
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
            var response = await GetRequest(UserInfoUrl);

            var sJObject = JObject.Parse(response);
            var userInfo = sJObject["response"][0];

            var userData = JsonConvert.DeserializeObject<VKUserdata>(userInfo.ToString());

            var city = userData.City.title;
            var country = userData.Country.title;

            return new User()
            {
                Name = new Name() { FirstName = (userData.first_name != null) ? userData.first_name : String.Empty, LastName = (userData.last_name != null) ? userData.last_name : String.Empty },
                ScreenName = (userData.screen_name != null) ? userData.screen_name : String.Empty,
                Gender = (userData.sex != 0) ? userData.sex.ToString() : String.Empty,
                Birthday = (userData.bdate != null) ? userData.bdate : String.Empty,
                Email = (email != null) ? email : String.Empty,
                Location =
                    new Location()
                    {
                        City = (city != null) ? city : String.Empty,
                        Country = (country != null) ? country : String.Empty
                    },
                Timezone = userData.timezone,
                Photo = userData.photo_max_orig

            };  
        }
    }
}