using System.IdentityModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Azimuth.Services;
using Azimuth.Shared.Enums;

namespace Azimuth.ApiControllers
{
    public class PlaylistsController : ApiController
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistsController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpGet]
        public HttpResponseMessage GetPublicPlaylists()
        {

            return Request.CreateResponse(HttpStatusCode.OK, _playlistService.GetPublicPlaylists());
        }

        [HttpPut]
        public HttpResponseMessage SetPlaylistAccessibilty(int playlistId, Accessibilty accessibilty)
        {
            try
            {
                _playlistService.SetAccessibilty(playlistId, accessibilty);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}