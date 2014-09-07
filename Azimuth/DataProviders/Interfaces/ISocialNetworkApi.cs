using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.Shared.Dto;

namespace Azimuth.DataProviders.Interfaces
{
    public interface ISocialNetworkApi
    {
        Task<List<TrackData.Audio>> GetTracks(string userId, string accessToken);
        Task<List<VkFriendData.Friend>>  GetFriendsInfo(string userId, string accessToken, int offset, int count);
        Task<List<VkTrackResponse.Audio>> GetSelectedTracks(DataForTrackSaving tracksInfo, string accessToken);
        Task<string> GetLyricsById(string userId, long lyricsId, string accessToken);
        Task<string[]> GetTrackLyricByArtistAndName(string artist, string trackName, string accessToken, string userId);
        Task<List<TrackData.Audio>> SearchTracksForLyric(List<TrackSearchInfo.SearchData> tracks, string accessToken);
        Task<List<TrackData.Audio>> SearchTracks(string searchText, string accessToken, byte inUserTracks);
        Task<string> AddTrack(string id, string audioId, string accessToken);
        Task<List<string>> GetTrackUrl(TrackSocialInfo tracks, string accessToken);
    }
}