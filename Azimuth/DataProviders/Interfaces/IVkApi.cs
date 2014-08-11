using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.Shared.Dto;

namespace Azimuth.DataProviders.Interfaces
{
    public interface IVkApi
    {
        Task<List<VKTrackData>> GetUserTracks(string userId, string accessToken);
        Task<VKTrackData> GetTrackById(string userId, string trackId, string accessToken);
        Task<List<VKTrackData>> GetTracksById(string userId, List<string> trackIds, string accessToken);
        Task<string> GetLyricsById(string userId, long lyricsId, string accessToken);
    }
}