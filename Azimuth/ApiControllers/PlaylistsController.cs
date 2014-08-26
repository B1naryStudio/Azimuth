﻿using System.IdentityModel;
using System;
﻿using System.Management.Instrumentation;
using System.Net;
using System.Net.Http;
﻿using System.Threading.Tasks;
﻿using System.Web.Http;
using Azimuth.Services;
using Azimuth.Shared.Enums;

namespace Azimuth.ApiControllers
{
    [RoutePrefix("api/playlists")]
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

        [HttpDelete]
        [Route("delete/{id:int}")]
        public HttpResponseMessage DeletePlaylistById(int id)
        {
            try
            {
                _playlistService.RemovePlaylistById(id);
                return Request.CreateResponse(HttpStatusCode.OK);
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

        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage DeleteTrackFromPlaylistById(int trackId, int playlistId)
        {
            try
            {
                _playlistService.RemoveTrackFromPlaylist(trackId, playlistId);
                return Request.CreateResponse(HttpStatusCode.OK);
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