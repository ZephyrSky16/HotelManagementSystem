using System.Web.Optimization;

namespace Revelation.Front.Office.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            var jquery = new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-3.7.0.js");
            var jqueryval = new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js");
            var modernizr = new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-2.8.3.js");
            var bootstrap = new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.bundle.js");
            var app = new ScriptBundle("~/bundles/app").Include("~/Scripts/app.js");
            var css = new StyleBundle("~/Content/css").Include(
                "~/Content/css/bootstrap.css",
                "~/Content/css/font-awesome.css",
                "~/Content/css/theme.css");

            // WebGrease's bundled minifier (last updated ~2013, shipped here because
            // it's on the approved package list) throws a NullReferenceException deep
            // in its JS parser on several of these files. Clearing each bundle's
            // transforms keeps bundling (fewer requests) while skipping minification
            // entirely, rather than depending on a known-broken minifier.
            foreach (var bundle in new Bundle[] { jquery, jqueryval, modernizr, bootstrap, app, css })
            {
                bundle.Transforms.Clear();
            }

            bundles.Add(jquery);
            bundles.Add(jqueryval);
            bundles.Add(modernizr);
            bundles.Add(bootstrap);
            bundles.Add(app);
            bundles.Add(css);
        }
    }
}
