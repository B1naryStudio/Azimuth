using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Infrastructure;
using Azimuth.Services;

namespace Azimuth.Controllers
{
    public class SettingsController : Controller
    {

        private ISettingsService _settingsService;
        private IAccountService _accountService;

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
            //_accountService.DisconnectUserAccount(provider, userId);
            return RedirectToAction("Index");
        }
    }
}