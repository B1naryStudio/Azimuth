using System;
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

        //public UserTracksController()
        //{
        //}

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
                var userSocialNetwork = userSocialNetworkRepo.Get(Convert.ToInt64(User.Identity.GetUserId()));

                tracks = VkApi.GetUserTracks(userSocialNetwork.ThirdPartId, userSocialNetwork.AccessToken);
                //tracks = VkApi.GetUserTracks("20167521", "79902b42d72f3178d66ede8971c6d4ff14d3bff1a23183cabae9d5ba6bd64ca348a203cadb8f402b7e7f2");

                _unitOfWork.Commit();
            }
            return Request.CreateResponse(HttpStatusCode.OK, tracks);
        }
    }
}
