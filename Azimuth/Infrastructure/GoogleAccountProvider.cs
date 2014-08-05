using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Location = Azimuth.DataAccess.Entities.Location;

namespace Azimuth.Infrastructure
{
    public class GoogleAccountProvider : AccountProvider
    {
        private readonly string _userId;
        private readonly string _accessToken;
        public string UserInfoUrl { get; private set; }

        public GoogleAccountProvider(string userId, string accessToken = "")
        {
            _userId = userId;
            _accessToken = accessToken;
            UserInfoUrl =
                String.Format(
                    @"https://www.googleapis.com/plus/v1/people/{0}?access_token={1}&fields=birthday%2CdisplayName%2Cemails%2Cgender%2Cimage%2Cname(familyName%2CgivenName)%2CplacesLived",
                    userId,
                    _accessToken);
        }

        public override async Task<User> GetUserInfoAsync(string email = "")
        {
            var response = await GetRequest(UserInfoUrl);

            var userInfo = JObject.Parse(response);

            var userData = JsonConvert.DeserializeObject<GoogleUserData>(userInfo.ToString());
            var myPlace = (userData.placesLived ?? new GoogleLocation[]{}).First(p => p.primary) ?? new GoogleLocation{value = String.Empty};
            var places = myPlace.value.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var city = places.First();
            var country = places.Last();

            return new User()
            {
                Name = new Name() { FirstName = userData.name.givenName?? String.Empty, LastName = userData.name.familyName ?? String.Empty },
                ScreenName = userData.displayName ?? String.Empty,
                Gender = userData.gender,
                Birthday = userData.birthday ?? String.Empty,
                Email = userData.emails.First((e)=>e.type.Equals("account")).value ?? String.Empty,
                Location =
                    new Location()
                    {
                        City = city ?? String.Empty,
                        Country = country ?? String.Empty
                    },
                Timezone = -100,
                Photo = userData.image.url

            };  
        }
    }
}