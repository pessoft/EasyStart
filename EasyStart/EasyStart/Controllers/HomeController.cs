using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using System.Web.Security;

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
            var result = new JsonResultModel
            {
                Success = true,
                URL = Url.Action("AdminPanel", "Admin")
            };

            return Json(result);
        }

        [HttpPost]
        public JsonResult Logout()
        {
            //FormsAuthentication.SignOut();

            var result = new JsonResultModel
            {
                Success = true,
                URL = Url.Action("AdminLogin", "Home")
            };

            return Json(result);
        }
    }
}