using System.Web.Mvc;
using Azimuth.Services;
using Azimuth.Services.Interfaces;

namespace Azimuth.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ISettingsService _settingsService;
        private readonly IAccountService _accountService;

        public SettingsController(ISettingsService settingsService, IAccountService accountService)
        {
            _settingsService = settingsService;
            _accountService = accountService;
        }

        public ActionResult Index()
        {
            var data = _settingsService.GetUserSettings();
            return View(data);
        }

        public ActionResult Disconnect(string provider)
        {
            _accountService.DisconnectUserAccount(provider);
            return RedirectToAction("Index");
        }
    }
}