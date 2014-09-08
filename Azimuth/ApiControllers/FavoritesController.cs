using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.Services.Concrete;

namespace Azimuth.ApiControllers
{
    [RoutePrefix("api/favorite")]
    public class FavoritesController : ApiController
    {
       private readonly IPlaylistLikesService _likesService;

       public FavoritesController(IPlaylistLikesService likesService)
        {
            _likesService = likesService;
        }


        [HttpGet]
        [Route("status/{id:int}")]
        public async Task<HttpResponseMessage> IsFavorite(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    _likesService.IsFavorite(id).Result);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

        }

        [HttpPost]
        [Route("{playlistId:int}")]
        public async Task<HttpResponseMessage> AddCurrentUserAsFavorite(int playlistId)
        {
            try
            {
                _likesService.AddCurrentUserAsFavorite(playlistId);
                return Request.CreateResponse(HttpStatusCode.OK, "Ok");
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [HttpDelete]
        [Route("{playlistId:int}")]
        public async Task<HttpResponseMessage> DeleteCurrentUserAsFavorite(int playlistId)
        {
            try
            {
                _likesService.RemoveCurrentUserAsFavorite(playlistId);
                return Request.CreateResponse(HttpStatusCode.OK, "Ok");
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}