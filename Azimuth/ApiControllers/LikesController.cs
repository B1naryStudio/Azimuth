using System.IdentityModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.Services.Concrete;

namespace Azimuth.ApiControllers
{
    [RoutePrefix("api/likes")]
    public class LikesController : ApiController
    {
        private readonly IPlaylistLikesService _likesService;

        public LikesController(IPlaylistLikesService likesService)
        {
            _likesService = likesService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id of playlist</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:int}/users")]
        public async Task<HttpResponseMessage> GetLikers(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _likesService.GetLikersByPlaylistId(id));
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
        public async Task<HttpResponseMessage> GetLikersAmount(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    _likesService.GetLikersByPlaylistId(id).Result.Count);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            
        }

        [HttpGet]
        [Route("status/{id:int}")]
        public async Task<HttpResponseMessage> DoYouLikeIt(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    _likesService.IsLiked(id).Result);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

        }

        [HttpPost]
        [Route("{playlistId:int}")]
        public async Task<HttpResponseMessage> AddCurrentUserAsLiker(int playlistId)
        {
            try
            {
                _likesService.AddCurrentUserAsLiker(playlistId);
                return Request.CreateResponse(HttpStatusCode.OK, "Ok");
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [HttpDelete]
        [Route("{playlistId:int}")]
        public async Task<HttpResponseMessage> DeleteCurrentUserAsLiker(int playlistId)
        {
            try
            {
                _likesService.RemoveCurrentUserAsLiker(playlistId);
                return Request.CreateResponse(HttpStatusCode.OK, "Ok");
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        
    }
}