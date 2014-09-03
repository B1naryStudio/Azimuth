using System.Threading.Tasks;
using Azimuth.Shared.Dto;

namespace Azimuth.DataProviders.Interfaces
{
    public interface IMusicService<T>
    {
        Task<T> GetTrackInfo(string author, string trackName);
    }
}