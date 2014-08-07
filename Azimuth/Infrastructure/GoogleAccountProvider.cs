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
        public GoogleAccountProvider(IWebClient webClient, string userId, string accessToken)
            :base(webClient)
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
            var response = await _webClient.GetWebData(UserInfoUrl);
            var userInfo = JObject.Parse(response);
            var userData = JsonConvert.DeserializeObject<GoogleUserData>(userInfo.ToString());

            var timezone = -100;
            var myPlace = ((userData.placesLived ?? new GoogleLocation[]{}).FirstOrDefault(p => p.primary) ?? new GoogleLocation{value = String.Empty}).value;
            var places = myPlace.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (!String.IsNullOrEmpty(myPlace))
            {
                var userLocCoordUrl = String.Format(
                    @"https://maps.googleapis.com/maps/api/geocode/json?address={0}",myPlace);
                response = await _webClient.GetWebData(userLocCoordUrl);
                var locInfo = JObject.Parse(response);
                if (locInfo["results"].Any())
                {
                    var locData = locInfo["results"][0];
                    var coordInfo = locData["geometry"]["location"];
                    var coord = new Tuple<string, string>(coordInfo["lat"].ToString(),
                        coordInfo["lng"].ToString());
                    var userTimezoneUrl = String.Format(
                        @"https://maps.googleapis.com/maps/api/timezone/json?location={0},{1}&timestamp=1331161200",
                        coord.Item1.Replace(',', '.'), coord.Item2.Replace(',', '.'));
                    response = await _webClient.GetWebData(userTimezoneUrl);
                    var timezoneInfo = JObject.Parse(response);
                    timezone = Int32.Parse(timezoneInfo["rawOffset"].ToString()) / 3600;
                }
            }
            string city = null;
            string country = null;
            if (places.Length == 1)
            {
                city = places.First();
            }
            if (places.Length > 1)
            {
                city = places.First();
                country = places.Last();
            }
            return new User
            {
                Name = new Name { FirstName = userData.name.givenName?? String.Empty, LastName = userData.name.familyName ?? String.Empty },
                ScreenName = userData.displayName ?? String.Empty,
                Gender = userData.gender,
                Birthday = userData.birthday ?? String.Empty,
                Email = userData.emails.FirstOrDefault(e=>e.type.Equals("account")).value ?? String.Empty,
                Location =
                    new Location
                    {
                        City = city ?? String.Empty,
                        Country = country ?? String.Empty
                    },
                Timezone = timezone,
                Photo = userData.image.url
            };;
        }
    }
}