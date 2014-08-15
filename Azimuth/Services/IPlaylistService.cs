
using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;

namespace Azimuth.Services
{
    public interface IPlaylistService
    {
        Task<List<PlaylistData>> GetPublicPlaylists();
    }
}