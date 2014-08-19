
using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Services
{
    public interface IPlaylistService
    {
        Task<List<PlaylistData>> GetPublicPlaylists();
        Task<List<PlaylistData>> GetUsersPlaylists();
        void SetAccessibilty(int id, Accessibilty accessibilty);
        Task<PlaylistData> GetPlaylistById(int id);
    }
}