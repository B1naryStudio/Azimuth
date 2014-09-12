﻿using System.Web.Optimization;

namespace Azimuth
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.clipboard.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/settings.css",
                      "~/Content/userprofile.css",
                      "~/Content/Loading.css",
                      "~/Content/jquery.mCustomScrollbar.css",
                      "~/Content/contextmenu.css",
                      "~/Content/draggable.css",
                      "~/Content/friends.css",
                      "~/Content/playlist.css",
                      "~/Content/track.css",
                      "~/Content/share.css",
                      "~/Content/site.css",
                      "~/Content/user.css"));
        }
    }
}
