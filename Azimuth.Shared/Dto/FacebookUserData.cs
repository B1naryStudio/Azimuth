using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class FacebookUserData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "birthday")]
        public string Birthday { get; set; }
        [JsonProperty(PropertyName = "timezone")]
        public int Timqzone { get; set; }
        [JsonProperty(PropertyName = "location")]
        public FbLocation Location { get; set; }
        [JsonProperty(PropertyName = "picture")]
        public FbPicture Picture { get; set; }

        public override string ToString()
        {
            return FirstName + LastName + Name + Gender + Email + Birthday + Timqzone +
                   Location.Name;
        }

        public class FbLocation
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
        }

        public class FbPicture
        {
            [JsonProperty(PropertyName = "data")]
            public Data Data { get; set; }
        }
        public class Data
        {
            [JsonProperty(PropertyName = "is_silhouette")]
            public bool IsSilhouette { get; set; }
            [JsonProperty(PropertyName = "url")]
            public string Url { get; set; }
        }
    }
}
