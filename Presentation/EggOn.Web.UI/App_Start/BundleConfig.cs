using EggOn.Web.UI.Utilities;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Optimization;

namespace FlowOptions.EggOn.WebApplication
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            var modulesPath = Path.GetFullPath(Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings.Get("ModulesPath")));

            BundleTable.VirtualPathProvider = new ModuleVirtualPathProvider(modulesPath, BundleTable.VirtualPathProvider);

            bundles.Add(new ScriptBundle("~/scripts/generic")
                .Include(
                    "~/Scripts/polyfill.js",
                    "~/Scripts/excanvas.js",
                    "~/Scripts/jquery-{version}.js",
                    "~/Scripts/jquery.flot.js",
                    "~/Scripts/jquery.flot.resize.js",
                    "~/Scripts/jquery.flot.time.js",
                    "~/Scripts/jquery.flot.pie.js",
                    "~/Scripts/moment.js",
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/bootstrap-colorpicker.js",
                    "~/Scripts/bootstrap-datetimepicker.js"
                )

                .Include(
                    "~/Scripts/angular.js",
                    "~/Scripts/AngularUI/ui-router.js",
                    "~/Scripts/angular-resource.js",
                    "~/Scripts/angular-sanitize.js",
                    "~/Scripts/ui-bootstrap-{version}.js",
                    "~/Scripts/ui-bootstrap-tpls-{version}.js"
                )
            );

            bundles.Add(new ScriptBundle("~/scripts/app")
                .Include("~/Areas/Core/Scripts/main.js", "~/Areas/Core/Scripts/config.js")
                .IncludeDirectory("~/Areas/", "*.js", true)
            //    .IncludeDirectory("~/Modules/", "*.js", true)
            );

            bundles.Add(new StyleBundle("~/styles/generic")
                .Include(
                    "~/Content/bootstrap.css",
                    "~/Content/bootstrap-theme.css",
                    "~/Scripts/angular-csp.css",
                    "~/Content/bootstrap-colorpicker.css",
                    "~/Content/bootstrap-datetimepicker.css",
                    "~/Content/jquery.treetable.css",
                    "~/Content/font-awesome.css"
                )
            );

            bundles.Add(new StyleBundle("~/styles/app")
                .IncludeDirectory("~/Areas/", "*.css", true)
            //    .IncludeDirectory("~/Modules/", "*.css", true)
            );
        }
    }
}