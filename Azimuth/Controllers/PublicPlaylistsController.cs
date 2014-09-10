using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Antlr.Runtime.Misc;
using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Concrete;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;

namespace Azimuth.Controllers
{
    public class PublicPlaylistsController : Controller
    {
        private readonly IPlaylistLikesService _playlistLikesService;
        private readonly IUserService _userService;
        private readonly IPlaylistService _playlistService;
        private readonly IUserTracksService _userTracksService;
        public PublicPlaylistsController(IPlaylistLikesService playlistLikesService, IUserService userService, IPlaylistService playlistService, IUserTracksService userTracksService)
        {
            _playlistLikesService = playlistLikesService;
            _userService = userService;
            _playlistService = playlistService;
            _userTracksService = userTracksService;
        }

        //
        // GET: /PublicPlaylists/
        public async Task<ActionResult> Index()
        {
            var user = _userService.GetUserInfo(AzimuthIdentity.Current.UserCredential.Id);
            return View(user);
        }

        [HttpPost]
        public PartialViewResult _ChangeLikeStatus (string playlistId, string isLiked, string isFavourited, string buttonType)
        {
            var playlist = Convert.ToInt32(playlistId);
            bool like = Convert.ToBoolean(isLiked);
            bool favourite = Convert.ToBoolean(isFavourited);
            if (buttonType == "like")
            {
                if (like)
                    _playlistLikesService.RemoveCurrentUserAsLiker(playlist);
                else
                    _playlistLikesService.AddCurrentUserAsLiker(playlist);

                ViewBag.isLiked = !like;
                ViewBag.isFavourited = favourite;
            }
            else
            {
                if (favourite)
                    _playlistLikesService.RemoveCurrentUserAsFavorite(playlist);
                else
                    _playlistLikesService.AddCurrentUserAsFavorite(playlist);

                ViewBag.isLiked = like;
                ViewBag.isFavourited = !favourite;
            }
            ViewBag.playlistId = playlistId;

            return PartialView();
        }

        public ActionResult NeedLogIn()
        {
            return RedirectToAction("Login", "Account");
        }

        public PartialViewResult _PublicPlaylists(ICollection<PlaylistLike> playlistFollowing)
        {
            var publicPlaylists = _playlistService.GetPublicPlaylistsSync();

            ViewData["playlistFollowing"] = playlistFollowing;

            return PartialView(publicPlaylists);
        }

        public PartialViewResult _PlaylistTracks(string playlistId, string playlistName, string isLiked, string isFavourited)
        {
            if (!String.IsNullOrEmpty(playlistId) || !String.IsNullOrWhiteSpace(playlistId))
            {
                var tracks = _userTracksService.GetTracksByPlaylistIdSync(Convert.ToInt32(playlistId));
                ViewBag.playlistId = playlistId;
                ViewBag.playlistName = playlistName;
                ViewBag.isLiked = Convert.ToBoolean(isLiked);
                ViewBag.isFavourited = Convert.ToBoolean(isFavourited);
                return PartialView(tracks);
            }
            return null;
        }
	}
}