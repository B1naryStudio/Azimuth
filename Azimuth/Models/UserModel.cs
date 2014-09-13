using System.Collections.Generic;
using System.Linq;
using Azimuth.DataAccess.Entities;

namespace Azimuth.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public Name Name { get; set; }
        public string ScreenName { get; set; }
        public string Photo { get; set; }
        public List<UserModel> Followers { get; set; }
        public List<UserModel> Following { get; set; }
        public List<PlaylistLike> PlaylistFollowing { get; set; }

        public static UserModel From(User user)
        {
            return new UserModel
            {
                Id = user.Id,
                Name = user.Name,
                ScreenName = user.ScreenName,
                Photo = user.Photo,
                Followers = user.Followers.Select(x => From(user)).ToList(),
                Following = user.Following.Select(x => From(user)).ToList(),
                PlaylistFollowing = user.PlaylistFollowing.ToList()
            };
        }
    }
}