using System;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Infrastructure.Interfaces;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;

namespace Azimuth.DataProviders.Concrete
{
    public class VkAccountProvider : AccountProvider
    {
        public string UserInfoUrl { get; private set; }

        public VkAccountProvider(IWebClient webClient, UserCredential userCredential)
            :base(webClient)
        {
            if (userCredential.AccessToken == null)
            {
                throw new ArgumentException("VKAccountProvider didn't receive Accesstoken");
            }
            if (userCredential.SocialNetworkId == null)
            {
                throw new ArgumentException("VKAccountProvider didn't receive SocialNetworkId");
            }

            var userId = userCredential.SocialNetworkId;
            var accessToken = userCredential.AccessToken;

            UserInfoUrl = String.Format(
                @"https://api.vk.com/method/users.get?user_id={0}&fields=screen_name,bdate,sex,city,country,photo_max_orig,timezone&v=5.23&access_token={1}",
                userId,
                accessToken);
        }

        public override async Task<User> GetUserInfoAsync(string email = "")
        {
            var response = await _webClient.GetWebData(UserInfoUrl);
            var userData = JsonConvert.DeserializeObject<VkUserData.VkResponse>(response);

            User currentUser = Mapper.Map(userData, new User());
            currentUser.Email = email ?? String.Empty;

            return currentUser;
        }
    }
}