using System.Web.Mvc;
using Azimuth.Infrastructure;

namespace Azimuth.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (AzimuthIdentity.Current != null && AzimuthIdentity.Current.IsAuthenticated)
            {
                return RedirectToAction("Index", "MusicList");
            }
            else
            {
                return View();
            }
        }
    }
}