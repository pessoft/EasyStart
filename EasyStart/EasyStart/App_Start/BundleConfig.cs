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
                        "~/Scripts/admin-panel-additional.js",
                        "~/Scripts/admin-panel.js",
                        "~/Scripts/product-constructor.js",
                        "~/Scripts/analytics.js"));

            bundles.Add(new ScriptBundle("~/bundles/timepicker").Include(
                        "~/Scripts/jquery-timepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-raty").Include(
                      "~/Scripts/jquery.raty.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
                      "~/Scripts/chart.js"));

            bundles.Add(new ScriptBundle("~/bundles/image-processing").Include(
                   "~/Scripts/cropper.js",
                   "~/Scripts/compressor.js",
                   "~/Scripts/image-processing.js"));

            bundles.Add(new ScriptBundle("~/bundles/promotion").Include(
                "~/Scripts/promotion/promotion.js",
                "~/Scripts/promotion/stock.js",
                "~/Scripts/promotion/coupon.js",
                "~/Scripts/promotion/cashback-partners.js",
                "~/Scripts/promotion/promotion-setting.js",
                "~/Scripts/promotion/push-notification.js",
                "~/Scripts/promotion/promotion-news.js"));

            bundles.Add(new ScriptBundle("~/bundles/product-additional-options").Include(
                   "~/Scripts/product-additional-options.js"));

            bundles.Add(new ScriptBundle("~/bundles/integration").Include(
                                "~/Scripts/integration/base-integration.js"));

            #region new logic
            bundles.Add(new ScriptBundle("~/bundles/event").Include(
                "~/Scripts/panel/event/event-type.js",
                "~/Scripts/panel/event/event-listener.js.js"));

            bundles.Add(new ScriptBundle("~/bundles/pages").Include(
                "~/Scripts/panel/pages/base-page-view.js",

                "~/Scripts/panel/pages/aboutUs/logic/about-us-page-logic.js",
                "~/Scripts/panel/pages/aboutUs/view/about-us-page-view.js",
                "~/Scripts/panel/pages/aboutUs/about-us-page.js",

                "~/Scripts/panel/pages/orders/logic/orders-page-logic.js",
                "~/Scripts/panel/pages/orders/view/orders-page-view.js",
                "~/Scripts/panel/pages/orders/orders-page.js",

                "~/Scripts/panel/pages/ordersHistory/logic/orders-history-page-logic.js",
                "~/Scripts/panel/pages/ordersHistory/view/orders-history-page-view.js",
                "~/Scripts/panel/pages/ordersHistory/orders-history-page.js",

                "~/Scripts/panel/pages/products/logic/products-page-logic.js",
                "~/Scripts/panel/pages/products/view/products-page-view.js",
                "~/Scripts/panel/pages/products/products-page.js",

                "~/Scripts/panel/pages/promotion/logic/promotion-page-logic.js",
                "~/Scripts/panel/pages/promotion/view/promotion-page-view.js",
                "~/Scripts/panel/pages/promotion/promotion-page.js",

                "~/Scripts/panel/pages/users/logic/users-page-logic.js",
                "~/Scripts/panel/pages/users/view/users-page-view.js",
                "~/Scripts/panel/pages/users/users-page.js",

                "~/Scripts/panel/pages/analytics/logic/analytics-page-logic.js",
                "~/Scripts/panel/pages/analytics/view/analytics-page-view.js",
                "~/Scripts/panel/pages/analytics/analytics-page.js",

                "~/Scripts/panel/pages/settings/logic/settings-page-logic.js",
                "~/Scripts/panel/pages/settings/view/settings-contacts-view.js",
                "~/Scripts/panel/pages/settings/view/settings-delivery-view.js",
                "~/Scripts/panel/pages/settings/view/settings-working-hours-view.js",
                "~/Scripts/panel/pages/settings/view/settings-order-payment-view.js",
                "~/Scripts/panel/pages/settings/view/settings-notification-view.js",
                "~/Scripts/panel/pages/settings/view/settings-integration-view.js",
                "~/Scripts/panel/pages/settings/view/settings-branch-view.js",
                "~/Scripts/panel/pages/settings/view/settings-page-view.js",
                "~/Scripts/panel/pages/settings/settings-page.js",

                "~/Scripts/panel/pages/tariffsAndPay/logic/tariffs-page-logic.js",
                "~/Scripts/panel/pages/tariffsAndPay/view/tariffs-page-view.js",
                "~/Scripts/panel/pages/tariffsAndPay/tariffs-page.js"));

            bundles.Add(new ScriptBundle("~/bundles/panel").Include(
                "~/Scripts/panel/notification/message-store.js",
                "~/Scripts/panel/navigator.js",
                "~/Scripts/panel/startup.js",
                "~/Scripts/panel/main.js"));
            #endregion
            bundles.Add(new StyleBundle("~/product-additional-options/css").Include(
                     "~/Content/product-additional-options.css"));

            bundles.Add(new StyleBundle("~/promotion/css").Include(
                     "~/Content/promotion.css"));

            bundles.Add(new StyleBundle("~/admin-login/css").Include(
                      "~/Content/admin-login.css"));

            bundles.Add(new StyleBundle("~/datepicker/css").Include(
                      "~/Content/datepicker.min.css"));

            bundles.Add(new StyleBundle("~/admin-panel/css").Include(
                      "~/Content/admin-panel.css",
                      "~/Content/admin-panel-additional.css",
                      "~/Content/product-constructor.css",
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

            bundles.Add(new StyleBundle("~/image-processing/css").Include(
                "~/Content/cropper.css",
                "~/Content/image-processing.css"));

            bundles.Add(new StyleBundle("~/integration/css").Include(
                "~/Content/integration.css"));

            bundles.Add(new StyleBundle("~/fontawesome/css").Include(
                   "~/Content/fontawesome/css/all.css"));

            bundles.Add(new StyleBundle("~/common/css").Include(
                   "~/Content/Panel/common.css"));
        }
    }
}
