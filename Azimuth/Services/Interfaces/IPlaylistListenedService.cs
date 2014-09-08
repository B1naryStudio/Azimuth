using System.Threading.Tasks;

namespace Azimuth.Services.Concrete
{
    public interface IPlaylistListenedService
    {
        Task<int> GetListenersAmount(int id);
        void AddNewListener(int playlistId);
    }
}