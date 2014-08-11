using System.Collections.Generic;
using System.Linq;
using Azimuth.DataAccess.Entities;

namespace Azimuth.Models
{
    public class UserModel
    {
        public Name Name { get; set; }
        public string ScreenName { get; set; }
        public string Photo { get; set; }
        public List<UserSocialNetwork> ConnectedSocialNetworks { get; set; }

        public static UserModel From(User user)
        {
            return new UserModel
            {
                Name = user.Name,
                ScreenName = user.ScreenName,
                Photo = user.Photo,
                ConnectedSocialNetworks = user.SocialNetworks.ToList()
            };
        }

        public static User ToUser(UserModel user)
        {
            return new User
            {
                Name = user.Name,
                ScreenName = user.ScreenName,
                Photo = user.Photo
            };
        }
    }
}