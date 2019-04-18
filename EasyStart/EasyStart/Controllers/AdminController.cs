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
            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            var typeBranch = DataWrapper.GetBranchType(branchId);
            var converter = new ConverterBranchSetting();
            var branchViewvs = converter.GetBranchSettingViews(
                DataWrapper.GetAllBranch(),
                DataWrapper.GetAllSettingDictionary(),
                typeBranch);

            ViewBag.BranchViews = branchViewvs != null && branchViewvs.Any() ? branchViewvs : null;
            ViewBag.City = CityHelper.City;
            ViewBag.Setting = null;
            ViewBag.TypeBranch = typeBranch;

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
            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
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
            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            var typeBranch = DataWrapper.GetBranchType(branchId);
            var newBranchId = DataWrapper.GetBranchId(newBranch.Login);

            if (typeBranch != TypeBranch.MainBranch)
            {
                result.ErrorMessage = "Вы не можете добавлять отделения";
                return Json(result);
            }

            var branch = new BranchModel
            {
                Login = newBranch.Login,
                Password = newBranch.Password,
                TypeBranch = Logic.TypeBranch.SubBranch
            };
            

            if (newBranchId == -1)
            {
                var success = DataWrapper.SaveBranch(branch);
                if (!success)
                {
                    result.ErrorMessage = "При сохранении что то пошло не так";
                }
                else
                {
                    newBranchId = DataWrapper.GetBranchId(newBranch.Login);
                    var setting = new SettingModel
                    {
                        BranchId = newBranchId,
                        CityId = newBranch.CityId
                    };

                    DataWrapper.SaveSetting(setting);

                    var converter = new ConverterBranchSetting();
                    var branchView = converter.GetBranchSettingViews(branch, setting, typeBranch);

                    result.Data = branchView;
                    result.Success = true;
                }
            }
            else
            {
                result.ErrorMessage = "Учетная запись с таким логином уже существует";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveBranch(int id)
        {
            var result = new JsonResultModel();
            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            var typeBranch = DataWrapper.GetBranchType(branchId);

            if (branchId == id)
            {
                result.ErrorMessage = "Самоудаление не возможно";
                return Json(result);
            }

            if (typeBranch != TypeBranch.MainBranch)
            {
                result.ErrorMessage = "Вы не можете удалять отделения";
            }
            else
            {
                var success = DataWrapper.RemoveBranch(id);

                if (success)
                {
                    result.Success = success;
                }
                else
                {
                    result.ErrorMessage = "Отдедение не удалено";
                }
            }
                
            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadBranchList()
        {
            var result = new JsonResultModel();

            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            var typeBranch = DataWrapper.GetBranchType(branchId);
            var converter = new ConverterBranchSetting();
            var branchViewvs = converter.GetBranchSettingViews(
                DataWrapper.GetAllBranch(),
                DataWrapper.GetAllSettingDictionary(),
                typeBranch);

            result.Data = branchViewvs;
            result.Success = true;

            return Json(result);
        }
    }
}