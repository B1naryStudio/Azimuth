using System.Web.Mvc;
using Azimuth.Services;

namespace Azimuth.Controllers
{
    public class MusicListController : Controller
    {
        public ActionResult Index()
        {
            //var data = _musicService.GetUSerMusiclList();


            return View();
        }
	}
}