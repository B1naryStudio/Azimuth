using System.Web.Mvc;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Migrations;


namespace Azimuth.Controllers
{
    public class TestController : Controller
    {
        private string cnString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Azimuth;Integrated Security=True;Pooling=False";

        //
        // GET: /Test/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RunMigrator()
        {
            Runner.RunMigrations(cnString, "Run migrations");
            return View();
        }

        public ActionResult Drop()
        {
            Runner.RunMigrations(cnString, "Drop all tables");
            return View();
        }
	}
}