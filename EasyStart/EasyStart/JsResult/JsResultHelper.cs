using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyStart.JsResult
{
    public static class JsResultHelper
    {
        public static JsResult JsResult(this Controller controller, object data, JsonRequestBehavior jsonRequestBehavior = JsonRequestBehavior.DenyGet)
        {
            return new JsResult(data, jsonRequestBehavior);
        }
    }
}