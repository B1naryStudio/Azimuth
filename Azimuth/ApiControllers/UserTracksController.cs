using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Shared.Dto;
using Iesi.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Azimuth.ApiControllers
{
    public class UserTracksController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVkApi _vkApi;
        private string _userThirdPartId;
        private string _userAccessToken;
        private long _userId;

        public UserTracksController(IUnitOfWork unitOfWork, IVkApi vkApi)
        {
            _unitOfWork = unitOfWork;
            _vkApi = vkApi;
        }

        public async Task<HttpResponseMessage> Get()
        {
            List<VKTrackData> tracks;
            using (_unitOfWork)
            {
                GetUserData();
                tracks =await _vkApi.GetUserTracks(_userThirdPartId, _userAccessToken);
                
                _unitOfWork.Commit();
            }
            return Request.CreateResponse(HttpStatusCode.OK, tracks);
        }

        public async Task<HttpResponseMessage> Post(PlaylistData playlistData)
        {
            using (_unitOfWork)
            {
                GetUserData();
                //var tracks = new HashedSet<Track>();
                var vkTracks =await _vkApi.GetTrackById(_userThirdPartId, playlistData.TrackIds[0], _userAccessToken);
                var l =  _vkApi.GetLyricsById(_userThirdPartId, vkTracks.LyricsId, _userAccessToken);
                //    foreach (var vkTrack in vkTracks)
                //    {
                //        tracks.Add(new Track
                //        {
                //            Duration = vkTrack.Duration,
                //        };
                //    }

                //    var userRepo = _unitOfWork.GetRepository<User>();
                //    var playlist = new Playlist
                //    {
                //        Name = playlistData.Name,
                //        Creator = userRepo.GetOne(s => s.Id == _userId),
                //        Tracks = tracks
                //    };

                //    var playlistRepo = _unitOfWork.GetRepository<Playlist>();
                //    playlistRepo.AddItem(playlist);

                //    _unitOfWork.Commit();
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private void GetUserData()
        {
            var userSocialNetworkRepo = _unitOfWork.GetRepository<UserSocialNetwork>();
            var userSocialNetwork = userSocialNetworkRepo.GetOne(s => s.ThirdPartId == User.Identity.GetUserId());
            _userThirdPartId = userSocialNetwork.ThirdPartId;
            _userAccessToken = userSocialNetwork.AccessToken;
            _userId = userSocialNetwork.Id;
        }
    }
}
