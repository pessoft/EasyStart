using EasyStart.Models;
using EasyStart.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyStart.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult AdminPanel()
        {
            ViewBag.City = CityHelper.City;
            return View();
        }

        [HttpPost]
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
                result.ErrorMEssage = "При загрузки изображения что то пошло не так";
            }
            
            return Json(result);
        }

        [HttpPost]
        public JsonResult SaveSetting(Setting setting)
        {
            var result = new JsonResultModel();
            return Json(result);
        }

        [HttpPost]
        public JsonResult AddCategory(CategoryModel category)
        {
            var result = new JsonResult();

            return Json(result);
        }

        [HttpPost]
        public JsonResult AddProduct(ProductModel product)
        {
            var result = new JsonResult();

            return Json(result);
        }

        [HttpPost]
        public JsonResult AddBranch(ProductModel product)
        {
            var result = new JsonResult();

            return Json(result);
        }
    }
}