using System.Web.Mvc;

namespace Azimuth.Controllers
{
    public class PublicPlaylistsController : Controller
    {
        //
        // GET: /PublicPlaylists/
        public ActionResult Index()
        {
            return View();
        }
	}
}