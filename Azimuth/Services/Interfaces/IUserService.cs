using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Azimuth.Shared.Dto;

namespace Azimuth.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<VkFriendData.Friend>> GetFriendsInfo(string provider, int offset, int count);
        Task<List<TrackData.Audio>> GetFriendsTracks(string provider, string friendId);
        UserDto GetUserInfo(int id);
        UserDto GetUserInfo(string email);
        Task<HttpResponseMessage> FollowPerson(long followerId);
    }
}