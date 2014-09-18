using System.Collections.Generic;
using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;

namespace Azimuth.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public Name Name { get; set; }
        public string ScreenName { get; set; }
        public string Photo { get; set; }
        public List<FollowerModel> Followers { get; set; }
        public List<FollowerModel> Following { get; set; }
        public List<PlaylistLikeModel> PlaylistFollowing { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public static UserModel From(User user)
        {
            return new UserModel
            {
                Id = user.Id,
                FirstName = user.Name.FirstName,
                LastName = user.Name.LastName,
                ScreenName = user.ScreenName,
                Photo = user.Photo,
                Followers = user.Followers.Select(s => new FollowerModel {Id = s.Id, FirstName = s.Name.FirstName, LastName = s.Name.LastName, ScreenName = s.ScreenName, Photo = s.Photo }).ToList(),
                Following = user.Following.Select(s => new FollowerModel { Id = s.Id, FirstName = s.Name.FirstName, LastName = s.Name.LastName, ScreenName = s.ScreenName, Photo = s.Photo }).ToList(),
                PlaylistFollowing = user.PlaylistFollowing
                                        .Select(pf => new PlaylistLikeModel
                                        {
                                            Name = pf.Playlist.Name,
                                            Id = pf.Playlist.Id,
                                            IsFavorite = pf.IsFavorite,
                                            IsLiked = pf.IsLiked
                                        }).ToList()
            };
        }
    }
}