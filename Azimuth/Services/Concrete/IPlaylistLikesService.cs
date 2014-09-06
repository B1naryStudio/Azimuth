using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;

namespace Azimuth.Services.Concrete
{
    public interface IPlaylistLikesService
    {
        Task<List<User>> GetLikersByPlaylistId(int id);
        void AddNewLike(int playlistId, int userId);
        void AddCurrentUserAsLiker(int playlistId);
        void RemoveCurrentUserAsLiker(int playlistId);
        void RemoveLike(int playlistId, int userId);
    }
}