using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;

namespace Azimuth.Services.Concrete
{
    public interface IPlaylistLikesService
    {
        Task<List<User>> GetLikersByPlaylistId(int id);
        void AddCurrentUserAsLiker(int playlistId);
        void AddCurrentUserAsFavorite(int playlistId);
        void RemoveCurrentUserAsLiker(int playlistId);
        void RemoveCurrentUserAsFavorite(int playlistId);
        Task<bool> IsLiked(int id);
        Task<bool> IsFavorite(int id);
    }
}