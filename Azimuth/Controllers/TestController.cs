using System.Web.Mvc;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Migrations;


namespace Azimuth.Controllers
{
    public class TestController : Controller
    {
#if DEBUG
        //private string cnString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Azimuth;Integrated Security=True;Pooling=False";
#else
        private string cnString = @"Server=b3b51a6b-d6c5-42a4-baca-a3840159ce32.sqlserver.sequelizer.com;Database=dbb3b51a6bd6c542a4bacaa3840159ce32;User ID=cvvuqikyozurbwuh;Password=rya6vs3LLyqqoAbVFJoFZbiHjwurBQT3AxN2dquT6ojodhSh3ahANryqqzV77Ji5;";
#endif

        private string cnString = @"Server=b3b51a6b-d6c5-42a4-baca-a3840159ce32.sqlserver.sequelizer.com;Database=dbb3b51a6bd6c542a4bacaa3840159ce32;User ID=cvvuqikyozurbwuh;Password=rya6vs3LLyqqoAbVFJoFZbiHjwurBQT3AxN2dquT6ojodhSh3ahANryqqzV77Ji5;";

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