using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;

namespace Azimuth.Services
{
    public interface IUserTracksService
    {
        Task<List<TrackData.Audio>> GetTracks(string provider);
		void SetPlaylist(PlaylistData playlistData, string provider);
        Task<ICollection<TracksDto>> GetUserTracks();
        void PutTrackToPlaylist(long playlistId, long trackId);
        void MoveTrackBetweenPlaylists(long playlistId, long trackId);
    }
}