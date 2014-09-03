
using System.Threading.Tasks;

namespace Azimuth.DataProviders.Interfaces
{
    public interface IChartLyricsApi
    {
        Task<string> GetTrackLyric(string author, string trackName);
    }
}