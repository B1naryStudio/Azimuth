using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.Shared.Dto;

namespace Azimuth.DataProviders.Interfaces
{
    public interface ISocialNetworkApi
    {
        Task<List<TrackData.Audio>> GetTracks(string userId, string accessToken);
        Task<List<VkFriendData.Friend>>  GetFriendsInfo(string userId, string accessToken, int offset, int count);
        Task<List<TrackData.Audio>> GetSelectedTracks(string userId, List<string> trackIds, string accessToken);
        Task<string> GetLyricsById(string userId, long lyricsId, string accessToken);
        Task<string[]> GetTrackLyricByArtistAndName(string artist, string trackName, string accessToken, string userId);
    }
}