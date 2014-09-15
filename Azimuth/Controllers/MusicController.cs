using System.Web.Mvc;

namespace Azimuth.Controllers
{
    public class MusicController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return View();   
            }
            return RedirectToAction("Login", "Account");
        }
	}
}