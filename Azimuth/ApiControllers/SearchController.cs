using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.Services.Interfaces;

namespace Azimuth.ApiControllers
{
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        private readonly IUserTracksService _userTracksService;
        private readonly IUserService _userService;

        public SearchController(IUserTracksService userTracksService, IUserService userService)
        {
            _userTracksService = userTracksService;
            _userService = userService;
        }

        [HttpGet]
        [Route("vkontakte")]
        public async Task<HttpResponseMessage> GetTracks(string searchText, int offset)
        {
            try
            {
                var data = await _userTracksService.VkontakteSearch(searchText, offset);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception);
            }
        }

        [HttpGet]
        [Route("users")]
        public async Task<HttpResponseMessage> GetUsers(string searchText)
        {
            try
            {
                var data = await _userService.SearchUsers(searchText);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception);
            }
        }
    }
}
