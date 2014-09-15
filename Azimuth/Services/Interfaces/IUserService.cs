using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;

namespace Azimuth.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<VkFriendData.Friend>> GetFriendsInfo(string provider, int offset, int count);
        Task<List<TrackData.Audio>> GetFriendsTracks(string provider, string friendId);
        User GetUserInfo(long id);
        User GetUserInfo(string email);
        User FollowPerson(long followerId);
        User UnfollowPerson(long followerId);
        Task<List<UserDto>> SearchUsers(string searchText);
    }
}