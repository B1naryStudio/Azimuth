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

        // Location classes
        public class AddressComponent
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public List<string> types { get; set; }
        }

        public class Northeast
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Southwest
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Bounds
        {
            public Northeast northeast { get; set; }
            public Southwest southwest { get; set; }
        }

        public class Location
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Northeast2
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Southwest2
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Viewport
        {
            public Northeast2 northeast { get; set; }
            public Southwest2 southwest { get; set; }
        }

        public class Geometry
        {
            public Bounds bounds { get; set; }
            public Location location { get; set; }
            public string location_type { get; set; }
            public Viewport viewport { get; set; }
        }

        public class Result
        {
            public List<AddressComponent> address_components { get; set; }
            public string formatted_address { get; set; }
            public Geometry geometry { get; set; }
            public List<string> types { get; set; }
        }

        public class LocationData
        {
            public List<Result> results { get; set; }
            public string status { get; set; }
        }

        public class Timezone
        {
            public int dstOffset { get; set; }
            public int rawOffset { get; set; }
            public string status { get; set; }
            public string timeZoneId { get; set; }
            public string timeZoneName { get; set; }
        }
    }
}
