using EasyStart.Logic;
using EasyStart.Models;
using EasyStart.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EasyStart.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        [Authorize]
        public ActionResult AdminPanel()
        {
            var branchId = DataWrapper.GetBranchIdBylogin(User.Identity.Name);
            ViewBag.City = CityHelper.City;
            ViewBag.Setting = null;

            if (branchId != -1)
            {
                ViewBag.Setting = DataWrapper.GetSetting(branchId);
            }
            
            return View();
        }

        [HttpPost]
        [Authorize]
        public JsonResult UploadImage()
        {
            var result = new JsonResultModel();

            try
            {
                foreach (string file in Request.Files)
                {
                    var upload = Request.Files[file];

                    if (upload != null)
                    {
                        string fileName = System.IO.Path.GetFileName(upload.FileName);
                        string ext = fileName.Substring(fileName.LastIndexOf("."));
                        string newFileName = String.Format(@"{0}{1}", System.Guid.NewGuid(), ext);

                        upload.SaveAs(Server.MapPath("~/images/" + newFileName));

                        result.Success = true;
                        result.URL = $"../images/{newFileName}";
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "При загрузки изображения что то пошло не так";
            }
            
            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveSetting(SettingModel setting)
        {
            var branchId = DataWrapper.GetBranchIdBylogin(User.Identity.Name);
            setting.BranchId = branchId;
            var successSave = DataWrapper.SaveSetting(setting);
            var result = new JsonResultModel();

            if (successSave)
            {
                result.Success = successSave;
            }
            else
            {
                result.ErrorMessage = "При сохранении натсройки что то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddCategory(CategoryModel category)
        {
            var result = new JsonResult();

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddProduct(ProductModel product)
        {
            var result = new JsonResult();

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddBranch(NewBranchModel newBranch)
        {
            var result = new JsonResultModel();
            var branchId = DataWrapper.GetBranchIdBylogin(newBranch.Login);
            var branch = new BranchModel
            {
                Login = newBranch.Login,
                Password = newBranch.Password,
                TypeBranch = Logic.TypeBranch.SubBranch
            };
            

            if (branchId == -1)
            {
                DataWrapper.SaveBranch(branch);
                branchId = DataWrapper.GetBranchIdBylogin(newBranch.Login);
                var setting = new SettingModel
                {
                    BranchId = branchId,
                    CityId = newBranch.CityId
                };

                DataWrapper.SaveSetting(setting);
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "Учетная запись с таким логином уже существует";
            }

            return Json(result);
        }
    }
}