
using System.Collections.Generic;

namespace Azimuth.Shared.Dto
{
    public class VKUserData
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public Sex sex { get; set; }
        public string screen_name { get; set; }
        public string bdate { get; set; }
        public City city { get; set; }
        public Country country { get; set; }
        public int timezone { get; set; }
        public string photo_max_orig { get; set; }
        public string email { get; set; }

        public enum Sex
        {
            none = 0,
            female = 1,
            male = 2
        }

        public class Country
        {
            public int id { get; set; }
            public string title { get; set; }
        }

        public class City
        {
            public int id { get; set; }
            public string title { get; set; }
        }

        public class Response
        {
            public List<VKUserData> response { get; set; }
        }
    }
}
