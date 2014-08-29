using System.Web.Mvc;
using Azimuth.Migrations;


namespace Azimuth.Controllers
{
    public class TestController : Controller
    {
#if DEBUG
        private const string CnString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Azimuth;Integrated Security=True;Pooling=False";
#else
        private string CnString = @"Server=b3b51a6b-d6c5-42a4-baca-a3840159ce32.sqlserver.sequelizer.com;Database=dbb3b51a6bd6c542a4bacaa3840159ce32;User ID=cvvuqikyozurbwuh;Password=rya6vs3LLyqqoAbVFJoFZbiHjwurBQT3AxN2dquT6ojodhSh3ahANryqqzV77Ji5;";
#endif

        //
        // GET: /Test/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RunMigrator()
        {
            Runner.RunMigrations(CnString, "Run migrations");
            return View();
        }

        public ActionResult Drop()
        {
            Runner.RunMigrations(CnString, "Drop all tables");
            return View();
        }
	}
}