using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Interfaces
{
    public interface IPlaylistService
    {
        Task<List<PlaylistData>> GetPublicPlaylists();
        List<PlaylistData> GetPublicPlaylistsSync(long? id);
        Task<List<PlaylistData>> GetUsersPlaylists();
        Task<List<PlaylistData>> GetUsersPlaylists(string id);
        Task<List<PlaylistData>> GetFavoritePlaylists();
        Task<List<PlaylistData>> GetNotOwnedFavoritePlaylists();
        void SetAccessibilty(int id, Accessibilty accessibilty);
        long CreatePlaylist(string name, Accessibilty accessibilty);
        Task<PlaylistData> GetPlaylistById(int id);
        void RemovePlaylistById(int id);
        void RemoveTrackFromPlaylist(int trackId, int playlistId);
        Task<string> GetImageById(int id);
        Task<string> GetSharedPlaylist(int playlistId);
        List<TracksDto> GetSharedTracks(string guid);
        Task<string> GetSharedPlaylist(List<long> tracksId);
        Task<string> SetPlaylistName(string azimuthPlaylist, string playlistName);
        Task<int> RaiseListenedCount(int id);
        Task<List<string>> GetPlaylistsGenres(long? id);
        List<PlaylistData> GetPublicPlaylistsSync(long? id, string genre);
        Task<string> SetPlaylistName(long id, string playlistName);
    }
}