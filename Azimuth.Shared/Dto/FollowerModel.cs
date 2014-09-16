
using Azimuth.DataAccess.Entities;

namespace Azimuth.Shared.Dto
{
    public class FollowerModel
    {
        public long Id { get; set; }
        public Name Name { get; set; }
        public string ScreenName { get; set; }
        public string Photo { get; set; }
    }
}
