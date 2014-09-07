using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.DataAccess.Entities;
using Azimuth.Services.Concrete;
using Azimuth.Services.Interfaces;

namespace Azimuth.ApiControllers
{
    [RoutePrefix("api/listeners")]
    public class ListenersController : ApiController
    {
        private readonly IListenersService _listenersService;

        public ListenersController(IListenersService playlistService)
        {
            _listenersService = playlistService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id of playlist</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:int}/users")]
        public async Task<HttpResponseMessage> GetListeners(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _listenersService.GetListenersByPlaylistId(id));
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id of playlist</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<HttpResponseMessage> GetListenersAmount(int id)
        {
            try
            {
                var test = _listenersService.GetListenersByPlaylistId(id).Result;
                return Request.CreateResponse(HttpStatusCode.OK, test.Count);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            
        }

        [HttpPost]
        [Route("{playlistId:int}/{userId:int}")]
        public async Task<HttpResponseMessage> AddListener(int playlistId, int userId)
        {
            try
            {
                _listenersService.AddNewListener(playlistId, userId);
                return Request.CreateResponse(HttpStatusCode.OK, "Ok");
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("{playlistId:int}")]
        public async Task<HttpResponseMessage> AddCurrentUserAsListener(int playlistId)
        {
            try
            {
                _listenersService.AddCurrentUserAsListener(playlistId);
                return Request.CreateResponse(HttpStatusCode.OK, "Ok");
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [HttpDelete]
        [Route("{playlistId:int}")]
        public async Task<HttpResponseMessage> DeleteCurrentUserAsListener(int playlistId)
        {
            try
            {
                _listenersService.RemoveCurrentUserAsListener(playlistId);
                return Request.CreateResponse(HttpStatusCode.OK, "Ok");
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}