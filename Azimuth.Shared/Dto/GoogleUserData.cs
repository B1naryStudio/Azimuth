using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimuth.Shared.Dto
{
    public class GoogleUserData
    {
        public GoogleName name { get; set; }
        public GoogleLocation[] placesLived { get; set; }
        public Photo image { get; set; }
        public string birthday { get; set; }
        public string gender { get; set; }
        public string displayName { get; set; }
        public Email[] emails { get; set; }

        public class Photo
        {
            public string url { get; set; }
        }
    }

    public class GoogleLocation
    {
        public string value { get; set; }
        public bool primary { get; set; }
    }

    public class Email
    {
        public string value { get; set; }
        public string type { get; set; }
    }

    public class GoogleName
    {
        public string formatted { get; set; }
        public string familyName { get; set; }
        public string givenName { get; set; }
        public string middleName { get; set; }
        public string honorifixicPrefix { get; set; }
        public string honorifixicSuffix { get; set; }
    }
}
