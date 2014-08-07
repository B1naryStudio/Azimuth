using System;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;

namespace Azimuth.Infrastructure
{
    public class FacebookAccountProvider: AccountProvider
    {
        private readonly string _accessToken;
        private readonly string _tokenExpiresIn;

        public string UserInfoUrl { get; private set; }

        public FacebookAccountProvider(IWebClient webClient, string userId, string accessToken)
            :base(webClient)
        {
            this._accessToken = accessToken;

            UserInfoUrl = String.Format(@"https://graph.facebook.com/v2.0/me?access_token={0}&fields=id,first_name,last_name,name,gender,email,birthday,timezone,location,picture.type(large)",
                    _accessToken);
        }

        public override async Task<User> GetUserInfoAsync(string email = "")
        {
            // Make query to Facebook Api
            var userDataJson = await _webClient.GetWebData(UserInfoUrl);
            var userData = JsonConvert.DeserializeObject<FacebookUserData>(userDataJson);

            string city = "", country = "", photoUrl = "";

            if (userData.location != null)
            {
                if (userData.location.name != null)
                {
                    city = userData.location.name.Split(',')[0];
                    country = userData.location.name.Split(' ')[1];
                }
            }

            return new User
            {
                Name = new Name { FirstName = userData.first_name ?? String.Empty, LastName = userData.last_name ?? String.Empty },
                ScreenName = userData.name ?? String.Empty,
                Gender = userData.gender ?? String.Empty,
                Birthday = userData.birthday ?? String.Empty,
                Email = email ?? String.Empty,
                Location =
                    new Location
                    {
                        City = city,
                        Country = country
                    },
                Timezone = userData.timezone,
                Photo = userData.picture.data.url
            };
        }
    }
}
