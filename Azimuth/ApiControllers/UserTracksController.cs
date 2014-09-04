﻿using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;

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
        [Route("trackinfo")]
        public async Task<HttpResponseMessage> GetTrackInfo(string artist, string trackName)
        {
            try
            {
                var data = await _userTracksService.GetTrackInfo(artist, trackName);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception);
            }
        }

        [HttpGet]
        [Route("tracksearch")]
        public async Task<HttpResponseMessage> SearchTracksInVk(string provider, string infoForSearch)
        {
            try
            {
                var listOfTracks = JsonConvert.DeserializeObject<TrackSearchInfo>(infoForSearch);
                var data = await _userTracksService.SearchTracksInSn(listOfTracks.TrackDatas, "Vkontakte");
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception);
            }
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
            catch (AccessDeniedException exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, exception);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(PlaylistData playlistData, string provider, int index, string friendId)
        {
            if (playlistData.TrackIds.Any())
            {
                await _userTracksService.SetPlaylist(playlistData, provider, index, friendId);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetUserTracks()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _userTracksService.GetUserTracks());
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetTracksByPlaylistId(int playlistId)
        {
            try
            {
                var tracks = await _userTracksService.GetTracksByPlaylistId(playlistId);
                return Request.CreateResponse(HttpStatusCode.OK, tracks);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPut]
        [Route("put")]
        public HttpResponseMessage UpdateTrackPlaylistPosition(long playlistId, int newIndex, List<long> trackId)
        {
            try
            {
                _userTracksService.UpdateTrackPlaylistPosition(playlistId, newIndex, trackId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("copy")]
        public HttpResponseMessage CopyTracksBetweenPlaylists(long playlistId, List<long> trackId)
        {
            try
            {
                _userTracksService.CopyTrackToAnotherPlaylist(playlistId, trackId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage DeleteTracksFromPlaylist(long playlistId, List<long> trackId)
        {
            try
            {
                _userTracksService.DeleteTracksFromPlaylist(playlistId, trackId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("move")]
        public async Task<HttpResponseMessage> MoveTracksBetweenPlaylists(long playlistId, List<long> trackId)
        {
            try
            {
                await _userTracksService.CopyTrackToAnotherPlaylist(playlistId, trackId);
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
