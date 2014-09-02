using System.Threading.Tasks;

namespace Azimuth.DataProviders.Interfaces
{
    public interface ILastfmApi
    {
        Task GetTrackInfo(string author, string trackName);
    }
}