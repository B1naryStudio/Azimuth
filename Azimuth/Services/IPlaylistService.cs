
using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Services
{
    public interface IPlaylistService
    {
        Task<List<PlaylistData>> GetPublicPlaylists();
        void SetAccessibilty(int id, Accessibilty accessibilty);
        Task<PlaylistData> GetPlaylistById(int id);
    }
}