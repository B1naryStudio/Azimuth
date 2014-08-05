using System;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Shared.Dto;
using Facebook;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject;

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

            var city = userData.Location.name.Split(',')[0];
            var country = userData.Location.name.Split(' ')[1];

            return new User()
            {
                Name = new Name() { FirstName = (userData.first_name != null) ? userData.first_name : String.Empty, LastName = (userData.last_name != null) ? userData.last_name : String.Empty },
                ScreenName = (userData.name != null) ? userData.name : String.Empty,
                Gender = (userData.gender != null) ? userData.gender : String.Empty,
                Birthday = (userData.birthday != null) ? userData.birthday : String.Empty,
                Email = (email != null) ? email : String.Empty,
                Location =
                    new DataAccess.Entities.Location()
                    {
                        City = (city != null) ? city : String.Empty,
                        Country = (country != null) ? country : String.Empty
                    },
                Timezone = userData.timezone,
                Photo = userDataObject["picture"]["data"]["url"].ToString()

            };
        }
    }
}
