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
        public ActionResult Index(string guid)
        {
            var tracks = _playlistService.GetSharedTracks(guid);

            return View(tracks);
        }
	}
}