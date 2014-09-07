using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Interfaces
{
    public interface IPlaylistService
    {
        Task<List<PlaylistData>> GetPublicPlaylists();
        Task<List<PlaylistData>> GetUsersPlaylists();
        Task<List<PlaylistData>> GetLikedPlaylists();
        Task<List<PlaylistData>> GetNotOwnedLikedPlaylists();
        void SetAccessibilty(int id, Accessibilty accessibilty);
        long CreatePlaylist(string name, Accessibilty accessibilty);
        Task<PlaylistData> GetPlaylistById(int id);
        void RemovePlaylistById(int id);
        void RemoveTrackFromPlaylist(int trackId, int playlistId);
        Task<string> GetImageById(int id);
    }
}