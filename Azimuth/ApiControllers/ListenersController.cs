using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public ICollection<User> GetListeners(int id)
        {
            return  _listenersService.GetListenersByPlaylistId(id).Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id of playlist</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:int}")]
        public int GetListenersAmount(int id)
        {
            return _listenersService.GetListenersByPlaylistId(id).Result.Count;
        }

        [HttpPost]
        [Route("{playlistId:int}/{userId:int}")]
        public void AddListener(int playlistId, int userId)
        {
            _listenersService.AddNewListener(playlistId, userId);
        }

        [HttpPost]
        [Route("{playlistId:int}")]
        public void AddCurrentUserAsListener(int playlistId)
        {
            _listenersService.AddCurrentUserAsListener(playlistId);
        }


        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}