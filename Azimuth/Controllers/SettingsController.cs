using System.Web.Mvc;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Interfaces;

namespace Azimuth.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ISettingsService _settingsService;
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        public SettingsController(ISettingsService settingsService, IAccountService accountService, IUserService userService)
        {
            _settingsService = settingsService;
            _accountService = accountService;
            _userService = userService;
        }

        public ActionResult Index()
        {
            ViewBag.Data = _settingsService.GetUserSettings();
            var user = _userService.GetUserInfo(AzimuthIdentity.Current.UserCredential.Id);
            return View(user);
        }

        public ActionResult Disconnect(string provider)
        {
            _accountService.DisconnectUserAccount(provider);
            return RedirectToAction("Index");
        }
    }
}