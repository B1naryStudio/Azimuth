using System.Collections.Generic;
using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class VkUserData
    {
        [JsonProperty(PropertyName = "id")]
        public int Id{ get; set; }
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "sex")]
        public VkSex Sex { get; set; }
        [JsonProperty(PropertyName = "screen_name")]
        public string ScreenName { get; set; }
        [JsonProperty(PropertyName = "bdate")]
        public string Birthday { get; set; }
        [JsonProperty(PropertyName = "city")]
        public VkCity City { get; set; }
        [JsonProperty(PropertyName = "country")]
        public VkCountry Country { get; set; }
        [JsonProperty(PropertyName = "timezone")]
        public int Timezone { get; set; }
        [JsonProperty(PropertyName = "photo_max_orig")]
        public string Photo { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        public enum VkSex
        {
            None = 0,
            Female = 1,
            Male = 2
        }

        public class VkCountry
        {
            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }
            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }
        }

        public class VkCity
        {
            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }
            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }
        }

        public class VkResponse
        {
            [JsonProperty(PropertyName = "response")]
            public List<VkUserData> Response { get; set; }
        }
    }
}
