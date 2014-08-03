using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimuth.Shared.Dto
{
    public class FacebookUserData
    {
        //[JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        //[JsonProperty(PropertyName = "first_name")]
        public string first_name { get; set; }
        //[JsonProperty(PropertyName = "last_name")]
        public string last_name { get; set; }
        //[JsonProperty(PropertyName = "email")]
        public string name { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        //[JsonProperty(PropertyName = "birthday")]
        public string birthday { get; set; }
        //[JsonProperty(PropertyName = "timezone")]
        public int timezone { get; set; }
        public Photo Photo { get; set; }
        public Location Location { get; set; }

        public override string ToString()
        {
            return first_name + last_name + name + gender + email + birthday + timezone +
                   Location.name;
        }
    }

    public class Location
    {
        //[JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        //[JsonProperty(PropertyName = "name")]
        public string name { get; set; }
    }

    public class Photo
    {
        //[JsonProperty(PropertyName = "url")]
        public string url { get; set; }
    }
    
}
