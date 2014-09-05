using System.Web.Mvc;
using Azimuth.Infrastructure.Concrete;

namespace Azimuth.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (AzimuthIdentity.Current != null && AzimuthIdentity.Current.IsAuthenticated)
            {
                return RedirectToAction("Index", "Music");
            }
            else
            {
                return View();
            }
        }
    }
}