using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Services;

namespace Azimuth.Controllers
{
    public class SettingsController : Controller
    {

        private ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public ActionResult Index()
        {
            var data = _settingsService.GetUserSettings();
            return View(data);
        }
	}
}