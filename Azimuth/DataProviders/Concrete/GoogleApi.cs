using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Shared.Dto;

namespace Azimuth.DataProviders.Concrete
{
    public class GoogleApi : ISocialNetworkApi
    {
        public Task<List<TrackData>> GetTracks(string userId, string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<TrackData> GetTrackById(string userId, string trackId, string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<TrackData>> GetTracksById(string userId, List<string> trackIds, string accessToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetLyricsById(string userId, long lyricsId, string accessToken)
        {
            throw new System.NotImplementedException();
        }
    }
}