using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.Services;
using Azimuth.Shared.Dto;

namespace Azimuth.ApiControllers
{
    public class UserTracksController : ApiController
    {
        private readonly IUserTracksService _userTracksService;

        public UserTracksController(IUserTracksService userTracksService)
        {
            _userTracksService = userTracksService;
        }

        public async Task<HttpResponseMessage> Get(string provider)
        {
            return Request.CreateResponse(HttpStatusCode.OK, await _userTracksService.GetTracks(provider));
        }

        public async Task<HttpResponseMessage> Post(PlaylistData playlistData)
        {
            //using (_unitOfWork)
            //{
            //    var tracks = new HashedSet<Track>();
            //    var vkTracks =await _vkApi.GetTrackById(_userThirdPartId, playlistData.TrackIds[0], _userAccessToken);
            //    var l =  _vkApi.GetLyricsById(_userThirdPartId, vkTracks.LyricsId, _userAccessToken);
            //        foreach (var vkTrack in vkTracks)
            //        {
            //            tracks.Add(new Track
            //            {
            //                Duration = vkTrack.Duration,
            //            };
            //        }

            //        var userRepo = _unitOfWork.GetRepository<User>();
            //        var playlist = new Playlist
            //        {
            //            Name = playlistData.Name,
            //            Creator = userRepo.GetOne(s => s.Id == _userId),
            //            Tracks = tracks
            //        };

            //        var playlistRepo = _unitOfWork.GetRepository<Playlist>();
            //        playlistRepo.AddItem(playlist);

            //        _unitOfWork.Commit();
            //}
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
