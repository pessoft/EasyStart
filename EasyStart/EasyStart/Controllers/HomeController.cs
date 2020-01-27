using EasyStart.Logic;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EasyStart.Controllers
{
    public class HomeController : Controller
    {
        private bool useState = false;

        public HomeController()
        {
            useState = UseMethod.GetCurrentState();
        }

        public ActionResult AdminLogin()
        {
            if(!DataWrapper.HasMainBranch())
            {
                var branch = new BranchModel
                {
                    Login = KeyGenerator.GetUniqueKey(6),
                    Password = KeyGenerator.GetUniqueKey(6),
                    TypeBranch = Logic.TypeBranch.MainBranch
                };

                var savedBranch = DataWrapper.SaveBranch(branch);

                new PromotionDefaultSetting(savedBranch.Id).SaveSettings();
            }

            if (User.Identity.IsAuthenticated)
            {
                if (!useState)
                {
                    FormsAuthentication.SignOut();
                }
                else
                {
                    return RedirectToAction("AdminPanel", "Admin");
                }
                
            }

            return View();
        }

        [HttpPost]
        public JsonResult Login(LoginDataModel loginData)
        {
            var result = new JsonResultModel();

            if (!useState)
            {
                result.ErrorMessage = "Сервис не оплачен";
                return Json(result);
            }

            var branch = DataWrapper.GetBranch(loginData.Login, loginData.Password);


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