﻿using System;
using System.IdentityModel;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Services;
using Azimuth.Shared.Dto;

namespace Azimuth.ApiControllers
{
    [RoutePrefix("api/usertracks")]
    public class UserTracksController : ApiController
    {
        private readonly IUserTracksService _userTracksService;

        public UserTracksController(IUserTracksService userTracksService)
        {
            _userTracksService = userTracksService;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Get(string provider)
        {
            try
            {
                var data = await _userTracksService.GetTracks(provider);
                return Request.CreateResponse(HttpStatusCode.OK, data);

            }
            catch (UserAuthorizationException exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, exception);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(PlaylistData playlistData, string provider)
        {
            _userTracksService.SetPlaylist(playlistData, provider);
            return Request.CreateResponse(HttpStatusCode.OK);
            //return Request.CreateResponse(correct ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetUserTracks()
        {
            return Request.CreateResponse(HttpStatusCode.OK, await _userTracksService.GetUserTracks());
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetTracksByPlaylistId(int playlistId)
        {
            try
            {
                var tracks = _userTracksService.GetTracksByPlaylistId(playlistId);
                return Request.CreateResponse(HttpStatusCode.OK, tracks);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPut]
        [Route("put")]
        public HttpResponseMessage PutTrackToPlaylist(int playlistId, int trackId)
        {
            try
            {
                _userTracksService.PutTrackToPlaylist(trackId, playlistId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // /api/usertracks/put?id=1
        [HttpPut]
        [Route("put")]
        public HttpResponseMessage PutTrack(int id, [FromBody] Track track)
        {
            try
            {
                _userTracksService.PutTrackToPlaylist(id, track);
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
