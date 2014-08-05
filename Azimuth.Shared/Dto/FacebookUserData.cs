using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimuth.Shared.Dto
{
    public class FacebookUserData
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string birthday { get; set; }
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
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Photo
    {
        public string url { get; set; }
    }
    
}
