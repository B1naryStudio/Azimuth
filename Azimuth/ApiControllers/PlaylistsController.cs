﻿﻿using System.IdentityModel;﻿
using System;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Http;
﻿using System.Threading.Tasks;
﻿using System.Web.Http;
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
        [Route("api/playlists/put/")]
        public HttpResponseMessage SetPlaylistAccessibilty(int id, Accessibilty accessibilty)
        {
            try
            {
                _playlistService.SetAccessibilty(id, accessibilty);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
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