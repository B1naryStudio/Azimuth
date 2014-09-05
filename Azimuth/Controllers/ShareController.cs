using System.Web.Mvc;
using Azimuth.Services.Interfaces;

namespace Azimuth.Controllers
{
    public class ShareController : Controller
    {
        private IPlaylistService _playlistService;
        public ShareController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }
        //
        // GET: /Share/
        public ActionResult Index(string id)
        {
            var tracks = _playlistService.GetSharedTracks(id);

            return View(tracks);
        }
	}
}