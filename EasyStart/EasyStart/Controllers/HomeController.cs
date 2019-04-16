using EasyStart.Logic;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
//using System.Web.Security;

namespace EasyStart.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult AdminLogin()
        {

            var branch = new BranchModel
            {
                Login = "login",
                Password = "password",
                TypeBranch = Logic.TypeBranch.MainBranch
            };

            DataWrapper.SaveBranch(branch);

            return View();
        }

        [HttpPost]
        public JsonResult Login(LoginDataModel loginData)
        {

            var branch = DataWrapper.GetBranch(loginData.Login, loginData.Password);
            var result = new JsonResultModel();

            if (branch != null)
            {
                FormsAuthentication.SetAuthCookie(branch.Login, true);
                result.Success = true;
                result.URL = Url.Action("AdminPanel", "Admin");
            }
            else
            {
                result.ErrorMessage = "Неверный логин или пароль";
            }


            return Json(result);
        }

        [HttpPost]
        public JsonResult Logout()
        {
            FormsAuthentication.SignOut();

            var result = new JsonResultModel
            {
                Success = true,
                URL = Url.Action("AdminLogin", "Home")
            };

            return Json(result);
        }
    }
}