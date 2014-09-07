using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Azimuth.Services.Concrete;

namespace Azimuth.ApiControllers
{
    [RoutePrefix("api/listened")]
    public class PlaylistListenedController : ApiController
    {
        private readonly IPlaylistListenedService _listenedService;

        public PlaylistListenedController(IPlaylistListenedService listenedService)
        {
            _listenedService = listenedService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id of playlist</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<HttpResponseMessage> GetLikes(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _listenedService.GetListenersAmount(id));
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPost]
        [Route("{playlistId:int}")]
        public async Task<HttpResponseMessage> AddNewListener(int playlistId)
        {
            try
            {
                _listenedService.AddNewListener(playlistId);
                return Request.CreateResponse(HttpStatusCode.OK, "Ok");
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

    }
}