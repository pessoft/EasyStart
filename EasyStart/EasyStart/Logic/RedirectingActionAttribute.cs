using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EasyStart.Logic
{
    public class RedirectingActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var useState = UseMethod.GetCurrentState();
            if (!useState)
            {

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.Clear();
                }

                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                         { "controller", "Home" },
                        { "action", "AdminLogin" }
                    });
            }
        }
    }
}