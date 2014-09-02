using System.Threading.Tasks;
using Azimuth.Shared.Dto;

namespace Azimuth.DataProviders.Interfaces
{
    public interface IDeezerService
    {
        Task<DeezerTrackData> GetTrackInfo(string author, string trackName);
    }
}
