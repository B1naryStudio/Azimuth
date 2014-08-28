using System;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azimuth.DataProviders.Concrete
{
    public class GoogleAccountProvider : AccountProvider
    {
        private readonly string _userId;
        private readonly string _accessToken;
        public string UserInfoUrl { get; private set; }
        public GoogleAccountProvider(IWebClient webClient, UserCredential userCredential)
            :base(webClient)
        {
            if (userCredential.SocialNetworkId == null)
            {
                throw new ArgumentException("GoogleAccountProvider didn't receive SocialNetworkId");
            }
            if (userCredential.AccessToken == null)
            {
                throw new ArgumentException("GoogleAccountProvider didn't receive AccessToken");
            }
            _userId = userCredential.SocialNetworkId;
            _accessToken = userCredential.AccessToken;
            UserInfoUrl =
                String.Format(
                    @"https://www.googleapis.com/plus/v1/people/{0}?access_token={1}&fields=birthday%2CdisplayName%2Cemails%2Cgender%2Cimage%2Cname(familyName%2CgivenName)%2CplacesLived",
                    _userId,
                    _accessToken);
        }

        public override async Task<User> GetUserInfoAsync(string email = "")
        {
            var response = await _webClient.GetWebData(UserInfoUrl);
            var userInfo = JObject.Parse(response);
            var userData = JsonConvert.DeserializeObject<GoogleUserData>(userInfo.ToString());

            var timezone = -100;
            var myPlace = ((userData.PlacesLived ?? new GoogleUserData.GoogleLocation[]{}).FirstOrDefault(p => p.Primary) ?? new GoogleUserData.GoogleLocation{Value = String.Empty}).Value;
            var places = myPlace.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (!String.IsNullOrEmpty(myPlace))
            {
                var userLocCoordUrl = String.Format(
                    @"https://maps.googleapis.com/maps/api/geocode/json?address={0}",myPlace);
                response = await _webClient.GetWebData(userLocCoordUrl);
                var locInfo = JObject.Parse(response);

                if (locInfo["results"].Any())
                {
                    var locData = JsonConvert.DeserializeObject<GoogleUserData.LocationData>(response);
                    var coordInfo = locData.Results.First().Geometry.Location;
                    var coord = new Tuple<string, string>(coordInfo.Lat.ToString(), coordInfo.Lat.ToString());
                    var userTimezoneUrl = String.Format(
                        @"https://maps.googleapis.com/maps/api/timezone/json?location={0},{1}&timestamp=1331161200",
                        coord.Item1.Replace(',', '.'), coord.Item2.Replace(',', '.'));
                    response = await _webClient.GetWebData(userTimezoneUrl);
                    var timezoneInfo = JsonConvert.DeserializeObject<GoogleUserData.Timezone>(response);
                    timezone = timezoneInfo.RawOffset / 3600;
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

            User currentUser = Mapper.Map(userData, new User());
            currentUser.Location = new Location
            {
                City = city ?? String.Empty,
                Country = country ?? String.Empty
            };
            currentUser.Location.Country = country ?? String.Empty;
            currentUser.Timezone = timezone;
            return currentUser;
        }
    }
}