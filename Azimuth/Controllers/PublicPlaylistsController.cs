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
        public async Task<ActionResult> Index(long? id, string genre)
        {
            ViewBag.Genre = genre;
            if (Request.IsAuthenticated)
            {
                var user = _userService.GetUserInfo(AzimuthIdentity.Current.UserCredential.Id);
                ViewBag.Id = id;
                return View(user);
            }
            return View();
        }

        [HttpPost]
        public PartialViewResult _ChangeLikeStatus(PublicPlaylistInfo curPlaylist, string buttonType)
        {
            if (buttonType == "like")
            {
                if (curPlaylist.IsLiked)
                {
                    _playlistLikesService.RemoveCurrentUserAsLiker(curPlaylist.PlaylistId);
                    curPlaylist.LikesCount--;
                }
                else
                {
                    _playlistLikesService.AddCurrentUserAsLiker(curPlaylist.PlaylistId);
                    curPlaylist.LikesCount++;
                }

                curPlaylist.IsLiked = !curPlaylist.IsLiked;
            }
            else
            {
                if (curPlaylist.IsFavourited)
                {
                    _playlistLikesService.RemoveCurrentUserAsFavorite(curPlaylist.PlaylistId);
                    curPlaylist.FavouritesCount--;
                }
                else
                {
                    _playlistLikesService.AddCurrentUserAsFavorite(curPlaylist.PlaylistId);
                    curPlaylist.FavouritesCount++;
                }

                curPlaylist.IsFavourited = !curPlaylist.IsFavourited;
            }

            return PartialView(new PublicPlaylistInfo
            {
                PlaylistId = curPlaylist.PlaylistId,
                PlaylistName = curPlaylist.PlaylistName,
                IsLiked = curPlaylist.IsLiked,
                IsFavourited = curPlaylist.IsFavourited,
                FavouritesCount = curPlaylist.FavouritesCount,
                LikesCount = curPlaylist.LikesCount
            });
            //return PartialView(curPlaylist);
        }

        public ActionResult NeedLogIn()
        {
            return RedirectToAction("Login", "Account");
        }

        public PartialViewResult _PublicPlaylists(ICollection<PlaylistLike> playlistFollowing, long? id, string genre)
        {
            List<PlaylistData> publicPlaylists = null;
            if (String.IsNullOrEmpty(genre) || genre == "All")
            {
                publicPlaylists = _playlistService.GetPublicPlaylistsSync(id);
            }
            else
            {
                publicPlaylists = _playlistService.GetPublicPlaylistsSync(id, genre);
            }
            ViewData["playlistFollowing"] = playlistFollowing;

            return PartialView(publicPlaylists);
        }

        public PartialViewResult _PlaylistTracks(string curPlaylist)
        {
            var playlist = JsonConvert.DeserializeObject<PublicPlaylistInfo>(curPlaylist);
            var tracks = _userTracksService.GetTracksByPlaylistIdSync(playlist.PlaylistId).ToList();
            var tracksViewModel = new PublicPlaylistTracksViewModel
            {
                Tracks = tracks,
                CurrentPlaylist = playlist
            };
                return PartialView(tracksViewModel);
        }
	}
}