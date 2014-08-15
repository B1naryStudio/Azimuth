using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Azimuth.Migrations;

namespace Azimuth.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Drop()
        {
            var dg = new DataGenerator.DataGenerator(MvcApplication.Container);
            dg.ClearDatabase();
            return null;
        }

        public ActionResult Create()
        {
            var dg = new DataGenerator.DataGenerator(MvcApplication.Container);
            dg.GenerateData();
            return null;
        }

        public ActionResult RunMigrator()
        {
            //string filePath = @"D:\Azimuth\Azimuth.Migrations\bin\Debug\Migrate.exe";
            ////string arguments = "--provider sqlserver2012 --a $(TargetName)$(TargetExt)";
            //string arguments = @"/connection Data Source=.\SQLEXPRESS;Initial Catalog=Azimuth;Integrated Security=True;Pooling=False D:\Azimuth\Azimuth.Migrations\bin\Debug\Azimuth.Migrations.dll";
            //Process.Start(filePath, arguments);

            string cnString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Azimuth;Integrated Security=True;Pooling=False";

            Migrations.Runner.MigrateToLatest(cnString);

            return null;
        }
	}
}