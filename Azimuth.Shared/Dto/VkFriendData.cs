using System.Collections.Generic;
using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class VkFriendData
    {
        [JsonProperty(PropertyName = "response")]
        public VkFriendResponse Response { get; set; }

        public class Friend
        {
            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }

            [JsonProperty(PropertyName = "first_name")]
            public string FirstName { get; set; }

            [JsonProperty(PropertyName = "last_name")]
            public string LastName { get; set; }

            [JsonProperty(PropertyName = "sex")]
            public VkUserData.VkSex Sex { get; set; }

            [JsonProperty(PropertyName = "screen_name")]
            public string ScreenName { get; set; }

            [JsonProperty(PropertyName = "bdate")]
            public string Birthday { get; set; }

            [JsonProperty(PropertyName = "city")]
            public VkUserData.VkCity City { get; set; }

            [JsonProperty(PropertyName = "country")]
            public VkUserData.VkCountry Country { get; set; }

            [JsonProperty(PropertyName = "photo_100")]
            public string Photo { get; set; }    
        }

        public class VkFriendResponse
        {
            [JsonProperty(PropertyName = "count")]
            public int Count { get; set; }
            [JsonProperty(PropertyName = "items")]
            public List<Friend> Friends { get; set; }
        }
    }
}