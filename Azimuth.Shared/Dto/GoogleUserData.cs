using System.Collections.Generic;
using Newtonsoft.Json;

namespace Azimuth.Shared.Dto
{
    public class GoogleUserData
    {
        [JsonProperty(PropertyName = "name")]
        public GoogleName Name { get; set; }
        [JsonProperty(PropertyName = "placesLived")]
        public GoogleLocation[] PlacesLived { get; set; }
        [JsonProperty(PropertyName = "image")]
        public Photo Image { get; set; }
        [JsonProperty(PropertyName = "birthday")]
        public string Birthday { get; set; }
        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "emails")]
        public Email[] Emails { get; set; }

        public class Photo
        {
            [JsonProperty(PropertyName = "url")]
            public string Url {get; set; }
        }

        public class GoogleLocation
        {
            [JsonProperty(PropertyName = "value")]
            public string Value { get; set; }
            [JsonProperty(PropertyName = "primary")]
            public bool Primary { get; set; }
        }

        public class Email
        {
            [JsonProperty(PropertyName = "value")]
            public string Value { get; set; }
            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }
        }

        public class GoogleName
        {
            [JsonProperty(PropertyName = "formatted")]
            public string Formatted { get; set; }
            [JsonProperty(PropertyName = "familyName")]
            public string FamilyName { get; set; }
            [JsonProperty(PropertyName = "givenName")]
            public string GivenName { get; set; }
            [JsonProperty(PropertyName = "middleName")]
            public string MiddleName { get; set; }
            [JsonProperty(PropertyName = "honorifixicPrefix")]
            public string HonorifixicPrefix { get; set; }
            [JsonProperty(PropertyName = "honorifixicSuffix")]
            public string HonorifixicSuffix { get; set; }
        }

        // Location classes
        public class AddressComponent
        {
            [JsonProperty(PropertyName = "long_name")]
            public string LongName { get; set; }
            [JsonProperty(PropertyName = "short_name")]
            public string ShortName { get; set; }
            [JsonProperty(PropertyName = "types")]
            public List<string> Types { get; set; }
        }

        public class Northeast
        {
            [JsonProperty(PropertyName = "lat")]
            public double Lat { get; set; }
            [JsonProperty(PropertyName = "lng")]
            public double Lng { get; set; }
        }

        public class Southwest
        {
            [JsonProperty(PropertyName = "lat")]
            public double Lat { get; set; }
            [JsonProperty(PropertyName = "lng")]
            public double Lng { get; set; }
        }

        public class Bounds
        {
            [JsonProperty(PropertyName = "northeast")]
            public Northeast NorthEast { get; set; }
            [JsonProperty(PropertyName = "southwest")]
            public Southwest SouthWest { get; set; }
        }

        public class Location
        {
            [JsonProperty(PropertyName = "lat")]
            public double Lat { get; set; }
            [JsonProperty(PropertyName = "lng")]
            public double Lng { get; set; }
        }

        public class Northeast2
        {
            [JsonProperty(PropertyName = "lat")]
            public double Lat { get; set; }
            [JsonProperty(PropertyName = "lng")]
            public double Lng { get; set; }
        }

        public class Southwest2
        {
            [JsonProperty(PropertyName = "lat")]
            public double Lat { get; set; }
            [JsonProperty(PropertyName = "lng")]
            public double Lng { get; set; }
        }

        public class Viewport
        {
            [JsonProperty(PropertyName = "northeast")]
            public Northeast2 NorthEast { get; set; }
            [JsonProperty(PropertyName = "southwest")]
            public Southwest2 SouthWest { get; set; }
        }

        public class Geometry
        {
            [JsonProperty(PropertyName = "bounds")]
            public Bounds Bounds { get; set; }
            [JsonProperty(PropertyName = "location")]
            public Location Location { get; set; }
            [JsonProperty(PropertyName = "location_type")]
            public string LocationType { get; set; }
            [JsonProperty(PropertyName = "viewport")]
            public Viewport Viewport { get; set; }
        }

        public class Result
        {
            [JsonProperty(PropertyName = "address_components")]
            public List<AddressComponent> AddressComponents { get; set; }
            [JsonProperty(PropertyName = "formatted_address")]
            public string FormattedAddress { get; set; }
            [JsonProperty(PropertyName = "geometry")]
            public Geometry Geometry { get; set; }
            [JsonProperty(PropertyName = "types")]
            public List<string> Types { get; set; }
        }

        public class LocationData
        {
            [JsonProperty(PropertyName = "results")]
            public List<Result> Results { get; set; }
            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }
        }

        public class Timezone
        {
            [JsonProperty(PropertyName = "dstOffset")]
            public int DstOffset { get; set; }
            [JsonProperty(PropertyName = "rawOffset")]
            public int RawOffset { get; set; }
            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }
            [JsonProperty(PropertyName = "timeZoneId")]
            public string TimezoneId { get; set; }
            [JsonProperty(PropertyName = "timeZoneName")]
            public string TimezoneName { get; set; }
        }
    }
}
