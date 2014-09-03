using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;

namespace Azimuth.Services.Interfaces
{
    public interface IUserTracksService
    {
        Task<List<TrackData.Audio>> GetTracks(string provider);
        Task<TrackInfoDto> GetTrackInfo(string author, string trackName);
        Task<DeezerTrackData> GetDeezerTrackInfo(string author, string trackName);
        Task<ICollection<TracksDto>> GetTracksByPlaylistId(int id);
        Task SetPlaylist(PlaylistData playlistData, string provider, int index, string friendId);
        Task<ICollection<TracksDto>> GetUserTracks();
        void UpdateTrackPlaylistPosition(long playlistId, int newIndex, List<long> trackId);
        void MoveTrackBetweenPlaylists(long playlistId, long trackId);
        void PutTrackToPlaylist(int id, Track track);
        Task CopyTrackToAnotherPlaylist(long playlistId, List<long> trackIds);
        Task DeleteTracksFromPlaylist(long playlistId, List<long> trackIds);
        Task<string> GetTrackLyrics(string author, string trackName);

    }
}