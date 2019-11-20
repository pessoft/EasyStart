using System.Web;
using System.Web.Optimization;

namespace EasyStart
{
    public class BundleConfig
    {
        // Дополнительные сведения об объединении см. на странице https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/odometer").Include(
                        "~/Scripts/odometer/odometer.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery-ui-1.12.1.min.js"));

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
                        "~/Scripts/admin-panel.js",
                        "~/Scripts/analytics.js"));

            bundles.Add(new ScriptBundle("~/bundles/timepicker").Include(
                        "~/Scripts/jquery-timepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-raty").Include(
                      "~/Scripts/jquery.raty.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
                      "~/Scripts/chart.js"));

            bundles.Add(new ScriptBundle("~/bundles/promotion").Include(
                "~/Scripts/promotion/promotion.js",
                "~/Scripts/promotion/stock.js",
                "~/Scripts/promotion/coupon.js"));

            bundles.Add(new StyleBundle("~/promotion/css").Include(
                     "~/Content/promotion.css"));

            bundles.Add(new StyleBundle("~/admin-login/css").Include(
                      "~/Content/admin-login.css"));

            bundles.Add(new StyleBundle("~/datepicker/css").Include(
                      "~/Content/datepicker.min.css"));

            bundles.Add(new StyleBundle("~/admin-panel/css").Include(
                      "~/Content/admin-panel.css",
                      "~/Content/analytics.css"));

            bundles.Add(new StyleBundle("~/timepicker/css").Include(
                      "~/Content/timePicker.css"));

            bundles.Add(new StyleBundle("~/dragula/css").Include(
                       "~/Content/dragula.css"));

            bundles.Add(new StyleBundle("~/sumoselect/css").Include(
                    "~/Content/sumoselect.min.css"));

            bundles.Add(new StyleBundle("~/main/css").Include(
                   "~/Content/main.css",
                   "~/Content/fontawesome/css/all.css",
                   "~/Content/material-input.css"));

            bundles.Add(new StyleBundle("~/odometer/css").Include(
                  "~/Content/odometer/odometer-theme-minimal.css"));
        }
    }
}
