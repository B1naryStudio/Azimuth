using System.Web.Mvc;
using Azimuth.Services.Interfaces;

namespace Azimuth.Controllers
{
    public class ShareController : Controller
    {
        private readonly IPlaylistService _playlistService;
        public ShareController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }
        //
        // GET: /Share/
        public ActionResult Index(string azimuth_playlist)
        {
            var tracks = _playlistService.GetSharedTracks(azimuth_playlist);

            return View(tracks);
        }
	}
}