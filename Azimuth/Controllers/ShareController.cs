using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Azimuth.Services.Interfaces;

namespace Azimuth.Controllers
{
    public class ShareController : Controller
    {
        private readonly IPlaylistService _playlistService;
        private IUserTracksService _userTracksService;

        public ShareController(IPlaylistService playlistService, IUserTracksService userTracksService)
        {
            _playlistService = playlistService;
            _userTracksService = userTracksService;
        }
        //
        // GET: /Share/
        public ActionResult Index(string azimuth_playlist)
        {
            var tracks = _playlistService.GetSharedTracks(azimuth_playlist);

            return View(tracks);
        }

        public async Task<ActionResult> Playlist(int id)
        {
            var tracks = (await _userTracksService.GetTracksByPlaylistId(id)).ToList();

            return View("Index", tracks);
        }
	}
}