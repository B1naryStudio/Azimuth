using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Azimuth.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Concrete;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;
using Newtonsoft.Json;

namespace Azimuth.Controllers
{
    public class RecommendationController : Controller
    {
        private readonly IPlaylistService _playlistService;
        private readonly IUserTracksService _userTracksService;
        private readonly IUserService _userService;

        public RecommendationController(IPlaylistService playlistService, IUserTracksService userTracksService, IUserService userService)
        {
            _playlistService = playlistService;
            _userTracksService = userTracksService;
            _userService = userService;
        }
        // GET: Recommendation
        public async Task<ActionResult> Index()
        {
            Dictionary<string, int> genres = new Dictionary<string, int>();

            if (Request.IsAuthenticated)
            {
                //var userPlaylists = await _playlistService.GetUsersPlaylists();
                //foreach (var playlist in userPlaylists)
                //{
                //    foreach (var genre in playlist.Genres)
                //    {
                //        var tracksCount = _userTracksService.GetTracks(t => t.)
                //    }
                //}

                var userTracks = _userTracksService.GetUserTracksSync();
                foreach (var track in userTracks)
                {
                    if (track.Genre == null || track.Genre == "Other" || track.Genre == "Undefined")
                    {
                        continue;
                    }
                    if (genres.Count > 0 && genres.ContainsKey(track.Genre))
                    {
                        genres[track.Genre]++;
                    }
                    else
                    {
                        genres.Add(track.Genre, 1);
                    }
                }

                genres = genres.OrderByDescending(i => i.Value, )

                //genres = genres.GroupBy(g => g.Key).OrderByDescending(o => o).Select(s => s).ToList();

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }


            return View();
        }
    }

    public class DictionaryComparer : IComparer<Dictionary<string, int>>
    {
        public int Compare(Dictionary<string, int> x, Dictionary<string, int> y)
        {
            if (x != null && y != null)
            {
                //if (x)
                //{
                    
                //}
            }
        }
    }
}