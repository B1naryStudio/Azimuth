using System;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azimuth.Infrastructure
{
    public class FacebookAccountProvider: AccountProvider
    {
        private readonly string _accessToken;
        private readonly string _tokenExpiresIn;

        public string UserInfoUrl { get; private set; }

        public FacebookAccountProvider(string userId, string accessToken)
        {
            this._accessToken = accessToken;

            UserInfoUrl = String.Format(@"https://graph.facebook.com/v2.0/me?access_token={0}&fields=id,first_name,last_name,name,gender,email,birthday,timezone,location,picture.type(large)",
                    _accessToken);
        }

        public override async Task<User> GetUserInfoAsync(string email = "")
        {
            // Make query to Facebook Api
            var userDataJson = await GetRequest(UserInfoUrl);
            var userDataObject = JObject.Parse(userDataJson);
            var userData = JsonConvert.DeserializeObject<FacebookUserData>(userDataJson);

            string city = "", country = "", photoUrl = "";

            if (userData.Location != null)
            {
                if (userData.Location.name != null)
                {
                    city = userData.Location.name.Split(',')[0];
                    country = userData.Location.name.Split(' ')[1];
                }
            }

            if (userDataObject["picture"] != null)
            {
                photoUrl = userDataObject["picture"]["data"]["url"].ToString();
            }
            

            return new User
            {
                Name = new Name { FirstName = userData.first_name ?? String.Empty, LastName = userData.last_name ?? String.Empty },
                ScreenName = userData.name ?? String.Empty,
                Gender = userData.gender ?? String.Empty,
                Birthday = userData.birthday ?? String.Empty,
                Email = email ?? String.Empty,
                Location =
                    new DataAccess.Entities.Location
                    {
                        City = city,
                        Country = country
                    },
                Timezone = userData.timezone,
                Photo = photoUrl
            };
        }
    }
}
