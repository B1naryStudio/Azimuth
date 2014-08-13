using System.Web.Mvc;
using Azimuth.Services;

namespace Azimuth.Controllers
{
    public class MusicListController : Controller
    {
        private IMusicService _musicService;

        public MusicListController(IMusicService musicService)
        {
            _musicService = musicService;
        }


        public ActionResult Index()
        {
            //var data = _musicService.GetUSerMusiclList();


            return View();
        }
	}
}