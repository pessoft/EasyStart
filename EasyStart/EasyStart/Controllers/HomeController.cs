using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyStart.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(LoginDataModel loginData)
        {
            var qwe = loginData;
            var result = new JsonResultModel
            {
                Success = true,
                URL = Url.Action("AdminPanel", "Admin")
            };

            return Json(result);
        }
    }
}