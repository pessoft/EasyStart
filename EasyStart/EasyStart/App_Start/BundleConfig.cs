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

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                        "~/Scripts/main.js",
                        "~/Scripts/loader.js",
                        "~/Scripts/maskedinput.min.js",
                        "~/Scripts/blocksIt.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin-login").Include(
                        "~/Scripts/admin-login.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin-panel").Include(
                        "~/Scripts/admin-panel.js"));

            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // готово к выпуску, используйте средство сборки по адресу https://modernizr.com, чтобы выбрать только необходимые тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/admin-login/css").Include(
                      "~/Content/admin-login.css",
                      "~/Content/fontawesome/css/all.css",
                      "~/Content/material-input.css"));

            bundles.Add(new StyleBundle("~/admin-panel/css").Include(
                      "~/Content/admin-panel.css",
                      "~/Content/fontawesome/css/all.css",
                      "~/Content/material-input.css"));
        }
    }
}
