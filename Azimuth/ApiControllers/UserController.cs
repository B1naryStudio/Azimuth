using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Interfaces;

namespace Azimuth.ApiControllers
{

    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var email = AzimuthIdentity.Current.UserCredential.Email;
            var data = _userService.GetUserInfo(email);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [Route("{id:int}")]
        public HttpResponseMessage GetById(int id)
        {
            var data = _userService.GetUserInfo(id);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [Route("friends/{provider:alpha}")]
        public async Task<HttpResponseMessage> GetUserFriendsInfo(string provider, int offset, int count)
        {
            var data = await _userService.GetFriendsInfo(provider, offset, count);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        [Route("friends/audio")]
        public async Task<HttpResponseMessage> GetFriendsTracks(string provider, string friendId)
        {
            var data = await _userService.GetFriendsTracks(provider, friendId);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPut]
        [Route("follow/{followerId:long}")]
        public HttpResponseMessage Follow(long followerId)
        {
            var updatedUser = _userService.FollowPerson(followerId);
            var userFollowers = updatedUser.Followers.Select(x => x.Id);
            return Request.CreateResponse(HttpStatusCode.OK, userFollowers);
        }

        [HttpPut]
        [Route("unfollow/{followerId:long}")]
        public HttpResponseMessage Unfollow(long followerId)
        {
            var updatedUser = _userService.UnfollowPerson(followerId);
            var userFollowers = updatedUser.Followers.Select(x => x.Id);
            return Request.CreateResponse(HttpStatusCode.OK, userFollowers);
        }
    }
}