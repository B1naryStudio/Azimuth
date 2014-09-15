using System.Web.Mvc;
using Azimuth.Services.Interfaces;

namespace Azimuth.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly ISettingsService _settingsService;

        public UserProfileController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        //
        // GET: /UserProfile/   
        public ActionResult Index(long? id)
        {
            if (Request.IsAuthenticated)
            {
                var settings = _settingsService.GetUserSettings(id);
                return View(settings);
            }
            return RedirectToAction("Login", "Account");
        }
    }
}