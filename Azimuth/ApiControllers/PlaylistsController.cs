﻿﻿using System.Collections.Generic;
﻿using System.IdentityModel;﻿
using System;
using System.Management.Instrumentation;
﻿using System.Net;
using System.Net.Http;
﻿using System.Threading.Tasks;
﻿using System.Web.Http;
﻿using Azimuth.Services.Interfaces;
﻿using Azimuth.Shared.Enums;
﻿
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
        public async Task<HttpResponseMessage> GetAllUsersPlaylists()
        {
            try
            {
                var data = await _playlistService.GetUsersPlaylists();
                return Request.CreateResponse(HttpStatusCode.OK, data);
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

        [HttpGet]
        [Route("public")]
        public async Task<HttpResponseMessage> GetPublicPlaylists()
        {
            try
            {
               return Request.CreateResponse(HttpStatusCode.OK, await _playlistService.GetPublicPlaylists());
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

        [HttpGet]
        [Route("own")]
        public async Task<HttpResponseMessage> GetOwnPlaylists()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _playlistService.GetUsersPlaylists());
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

        [HttpGet]
        [Route("friend")]
        public async Task<HttpResponseMessage> GetFriendsPlaylists(string id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _playlistService.GetUsersPlaylists(id));
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

        [HttpGet]
        [Route("genres")]
        public async Task<HttpResponseMessage> GetPlaylistsGenres()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _playlistService.GetPlaylistsGenres());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetPlayListById(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _playlistService.GetPlaylistById(id));
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

        [HttpGet]
        [Route("favorite")]
        public async Task<HttpResponseMessage> GetFavorites()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _playlistService.GetFavoritePlaylists());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet]
        [Route("favorite/notOwned")]
        public async Task<HttpResponseMessage> GetNowOwnedFavorites()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _playlistService.GetNotOwnedFavoritePlaylists());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet]
        [Route("image/{id:int}")]
        public async Task<HttpResponseMessage> GetPlaylistImageById(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _playlistService.GetImageById(id));
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

        [HttpGet]
        [Route("share/{id:int}")]
        public async Task<HttpResponseMessage> GetSharedPlaylist(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _playlistService.GetSharedPlaylist(id));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);

            }
        }

        [HttpPut]
        [Route("share")]
        public async Task<HttpResponseMessage> GetSharedPlaylist(List<long> tracksId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _playlistService.GetSharedPlaylist(tracksId));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet]
        [Route("share")]
        public async Task<HttpResponseMessage> SetPlaylistName(string azimuthPlaylist, string playlistName)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _playlistService.SetPlaylistName(azimuthPlaylist, playlistName));
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
                return Request.CreateResponse(HttpStatusCode.OK,"it's ok");
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

        [HttpPost]
        public HttpResponseMessage CreatePlaylist(string name, Accessibilty accessibilty)
        {
            try
            {
                long playlistId = _playlistService.CreatePlaylist(name, accessibilty);
                return Request.CreateResponse(HttpStatusCode.OK, playlistId);
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("raiselistened")]
        public async  Task<HttpResponseMessage> RaiseListenedCount(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _playlistService.RaiseListenedCount(id));
            }
            catch (BadRequestException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        
        [HttpPut]
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
    }
}