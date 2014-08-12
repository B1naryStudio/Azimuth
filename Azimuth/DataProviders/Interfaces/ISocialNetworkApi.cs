using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.Shared.Dto;

namespace Azimuth.DataProviders.Interfaces
{
    public interface ISocialNetworkApi
    {
        Task<List<TrackData>> GetTracks(string userId, string accessToken);
        Task<TrackData> GetTrackById(string userId, string trackId, string accessToken);
        Task<List<TrackData>> GetTracksById(string userId, List<string> trackIds, string accessToken);
        Task<string> GetLyricsById(string userId, long lyricsId, string accessToken);
    }
}