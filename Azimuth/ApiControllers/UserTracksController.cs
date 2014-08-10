using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataProviders.Concrete;
using Azimuth.Shared.Dto;
using Microsoft.AspNet.Identity;

namespace Azimuth.ApiControllers
{
    public class UserTracksController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserTracksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public HttpResponseMessage Get()
        {
            List<VKTrackData> tracks;
            using (_unitOfWork)
            {
                IRepository<UserSocialNetwork> userSocialNetworkRepo = _unitOfWork.GetRepository<UserSocialNetwork>();
                var userSocialNetwork = userSocialNetworkRepo.GetOne(s => s.ThirdPartId == User.Identity.GetUserId());
                
                tracks = VkApi.GetUserTracks(userSocialNetwork.ThirdPartId, userSocialNetwork.AccessToken);
                
                _unitOfWork.Commit();
            }
            return Request.CreateResponse(HttpStatusCode.OK, tracks);
        }
    }
}
