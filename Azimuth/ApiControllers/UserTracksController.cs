using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Shared.Dto;
using Microsoft.AspNet.Identity;

namespace Azimuth.ApiControllers
{
    public class UserTracksController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVkApi _vkApi;

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
                IRepository<UserSocialNetwork> userSocialNetworkRepo = _unitOfWork.GetRepository<UserSocialNetwork>();
                var userSocialNetwork = userSocialNetworkRepo.GetOne(s => s.ThirdPartId == User.Identity.GetUserId());

                tracks =await _vkApi.GetUserTracks(userSocialNetwork.ThirdPartId, userSocialNetwork.AccessToken);
                
                _unitOfWork.Commit();
            }
            return Request.CreateResponse(HttpStatusCode.OK, tracks);
        }
    }
}
