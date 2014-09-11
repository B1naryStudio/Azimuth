using System;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Infrastructure.Interfaces;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;

namespace Azimuth.DataProviders.Concrete
{
    public class FacebookAccountProvider: AccountProvider
    {
        public string UserInfoUrl { get; private set; }

        public FacebookAccountProvider(IWebClient webClient, UserCredential userCredential)
            :base(webClient)
        {
            if (userCredential.AccessToken == null)
            {
                throw new ArgumentException("FacebookAccountProvider didn't receive accessToken");
            }

            var accessToken = userCredential.AccessToken;
            UserInfoUrl = String.Format(@"https://graph.facebook.com/v2.0/me?access_token={0}&fields=id,first_name,last_name,name,gender,email,birthday,timezone,location,picture.type(large)",
                    accessToken);
        }

        public override async Task<User> GetUserInfoAsync(string email = "")
        {
            // Make query to Facebook Api
            var userDataJson = await _webClient.GetWebData(UserInfoUrl);
            var userData = JsonConvert.DeserializeObject<FacebookUserData>(userDataJson);

            return Mapper.Map(userData, new User());
        }
    }
}
