
using System;
using System.Data.SqlClient;
using System.IdentityModel;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.Services;

namespace Azimuth.ApiControllers
{
    public class PlaylistsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private IPlaylistService _playlistService;

        public PlaylistsController(IUnitOfWork unitOfWork, IPlaylistService playlistService)
        {
            _unitOfWork = unitOfWork;

            _playlistService = playlistService;
        }

        public async Task<HttpResponseMessage> GetPublicPlaylists()
        {

            return Request.CreateResponse(HttpStatusCode.OK, _playlistService.GetPublicPlaylists());
        }

        public async Task<HttpResponseMessage> GetPlayListById(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _playlistService.GetPlaylistById(id));
            }
            catch (InstanceNotFoundException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}