using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;

namespace Azimuth.ApiControllers
{

    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private IUserService _userService;

        public UserController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            // TODO: Rework with userService
            //User user = null;
            //using (_unitOfWork)
            //{
            //    IRepository<User> userRepo = _unitOfWork.GetRepository<User>();
            //    user = userRepo.Get(id);
                
            //    _unitOfWork.Commit();
            //}

            //var dto = new UserBrief
            //{
            //    Name = user.ScreenName,
            //    Email = user.Email
            //};

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("friends/{provider:alpha}")]
        public async Task<HttpResponseMessage> GetUserFriendsInfo(string provider)
        {
            var data = await _userService.GetFriendsInfo(provider);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [Route("friends/audio")]
        public async Task<HttpResponseMessage> GetFriendsTracks(string provider, string friendId)
        {
            var data = await _userService.GetFriendsTracks(provider, friendId);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}