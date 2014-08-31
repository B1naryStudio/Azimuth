using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;

namespace Azimuth.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<VkFriendData.Friend>> GetFriendsInfo(string provider);
        Task<List<TrackData.Audio>> GetFriendsTracks(string provider, string friendId);
        UserDto GetUserInfo(int id);
        UserDto GetUserInfo(string email);
    }
}