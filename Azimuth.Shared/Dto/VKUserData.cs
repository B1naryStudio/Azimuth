
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class VKUserData
    {
        [JsonProperty(PropertyName = "id")]
        public int Id{ get; set; }
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "sex")]
        public VKSex Sex { get; set; }
        [JsonProperty(PropertyName = "screen_name")]
        public string ScreenName { get; set; }
        [JsonProperty(PropertyName = "bdate")]
        public string Birthday { get; set; }
        [JsonProperty(PropertyName = "city")]
        public VKCity City { get; set; }
        [JsonProperty(PropertyName = "country")]
        public VKCountry Country { get; set; }
        [JsonProperty(PropertyName = "timezone")]
        public int Timezone { get; set; }
        [JsonProperty(PropertyName = "photo_max_orig")]
        public string Photo { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        public enum VKSex
        {
            none = 0,
            female = 1,
            male = 2
        }

        public class VKCountry
        {
            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }
            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }
        }

        public class VKCity
        {
            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }
            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }
        }

        public class VKResponse
        {
            [JsonProperty(PropertyName = "response")]
            public List<VKUserData> Response { get; set; }
        }
    }
}
