using System.Threading.Tasks;
using Azimuth.Shared.Dto;

namespace Azimuth.DataProviders.Interfaces
{
    public interface IMusicService
    {
        Task<TrackInfoDto> GetTrackInfo(string author, string trackName);
    }
}