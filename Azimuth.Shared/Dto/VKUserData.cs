
namespace Azimuth.Shared.Dto
{
    public class VKUserdata
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public City City { get; set; }
        public Country Country { get; set; }
        public string photo_max_orig { get; set; }
        public string bdate { get; set; }
        public Sex sex { get; set; }
        public int timezone { get; set; }
        public string screen_name { get; set; }
        public string email { get; set; }
    }

    public enum Sex
    {
        none = 0,
        female = 1,
        male = 2
    }

    public class Country
    {
        public string title { get; set; }
    }

    public class City
    {
        public string title { get; set; }
    }
}
