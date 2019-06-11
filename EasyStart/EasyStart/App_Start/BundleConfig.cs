using System.Web;
using System.Web.Optimization;

namespace EasyStart
{
    public class BundleConfig
    {
        // Дополнительные сведения об объединении см. на странице https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/sumoselect").Include(
                         "~/Scripts/jquery.sumoselect.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
                         "~/Scripts/jquery.signalR-2.4.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                        "~/Scripts/main.js",
                        "~/Scripts/loader.js",
                        "~/Scripts/maskedinput.min.js",
                        "~/Scripts/blocksIt.js"));

            bundles.Add(new ScriptBundle("~/bundles/notify").Include(
                      "~/Scripts/notify.js"));

            bundles.Add(new ScriptBundle("~/bundles/dragula").Include(
                        "~/Scripts/dragula.js"));

            bundles.Add(new ScriptBundle("~/bundles/darepicker").Include(
                        "~/Scripts/datepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin-login").Include(
                        "~/Scripts/admin-login.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin-panel").Include(
                        "~/Scripts/admin-panel.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery-raty").Include(
                      "~/Scripts/jquery.raty.min.js"));

            bundles.Add(new StyleBundle("~/admin-login/css").Include(
                      "~/Content/admin-login.css"));
            bundles.Add(new StyleBundle("~/datepicker/css").Include(
                      "~/Content/datepicker.min.css"));

            bundles.Add(new StyleBundle("~/admin-panel/css").Include(
                      "~/Content/admin-panel.css"));

            bundles.Add(new StyleBundle("~/dragula/css").Include(
                       "~/Content/dragula.css"));

            bundles.Add(new StyleBundle("~/sumoselect/css").Include(
                    "~/Content/sumoselect.min.css"));

            bundles.Add(new StyleBundle("~/main/css").Include(
                   "~/Content/main.css",
                   "~/Content/fontawesome/css/all.css",
                   "~/Content/material-input.css"));
        }
    }
}
