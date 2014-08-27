using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;

namespace Azimuth.Services
{
    public interface IUserTracksService
    {
        Task<List<TrackData.Audio>> GetTracks(string provider);

        Task<ICollection<TracksDto>> GetTracksByPlaylistId(int id);
		void SetPlaylist(PlaylistData playlistData, string provider);
        Task<ICollection<TracksDto>> GetUserTracks();
        void PutTrackToPlaylist(long trackId, long playlistId);
        void PutTrackToPlaylist(int id, Track track);

    }
}