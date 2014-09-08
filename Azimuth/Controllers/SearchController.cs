using System.Web.Mvc;

namespace Azimuth.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/
        public ActionResult Index(string searchParam)
        {
            return View();
        }
	}
}