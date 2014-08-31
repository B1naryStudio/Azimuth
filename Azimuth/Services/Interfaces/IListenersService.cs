using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;

namespace Azimuth.ApiControllers
{
    public interface IListenersService
    {
        Task<List<User>> GetListenersByPlaylistId(int id);
        void AddNewListener(int playlistId, int userId);
        void AddCurrentUserAsListener(int playlistId);
        void RemoveListener(int playlistId, int userId);
        
    }
}