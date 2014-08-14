﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Services;
using Azimuth.Shared.Dto;
using Azimuth.ViewModels;
using Microsoft.Ajax.Utilities;

namespace Azimuth.ApiControllers
{
    public class UserTracksController : ApiController
    {
        private readonly IUserTracksService _userTracksService;

        public UserTracksController(IUserTracksService userTracksService)
        {
            _userTracksService = userTracksService;
        }

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

        public async Task<HttpResponseMessage> Post(PlaylistData playlistData, string provider)
        {
            _userTracksService.SetPlaylist(playlistData, provider);
            return Request.CreateResponse(HttpStatusCode.OK);
            //return Request.CreateResponse(correct ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        }

        public async Task<HttpResponseMessage> GetUserTracks()
        {
            return Request.CreateResponse(HttpStatusCode.OK, await _userTracksService.GetUserTracks());
        }
    }
}
