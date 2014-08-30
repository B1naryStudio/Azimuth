using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.Shared.Dto;

namespace Azimuth.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<VkFriendData.Friend>> GetFriendsInfo(string provider);
        Task<List<TrackData.Audio>> GetFriendsTracks(string provider, string friendId);
    }
}