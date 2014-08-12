using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.Shared.Dto;

namespace Azimuth.Services
{
    public interface IUserTracksService
    {
        Task<List<TrackData>> GetTracks(string provider);
    }
}