using EasyStart.Logic;
using EasyStart.Models;
using EasyStart.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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

            ViewBag.Zones = DateTimeHepler.GetDisplayDictionary();

            ViewBag.BranchViews = branchViewvs != null && branchViewvs.Any() ? branchViewvs : null;
            ViewBag.City = CityHelper.City;
            ViewBag.Setting = null;
            ViewBag.DeliverySetting = null;
            ViewBag.DeliveryTimeTable= null;
            ViewBag.TypeBranch = typeBranch;
            //branchId = -1;
            if (branchId != -1)
            {
                var deliverySetting = DataWrapper.GetDeliverySetting(branchId); ;
                ViewBag.ZoneId = deliverySetting.ZoneId;
                ViewBag.Setting = DataWrapper.GetSetting(branchId);
                ViewBag.DeliverySetting = deliverySetting;
                ViewBag.DeliveryTimeTable = WeeklyDayHelper.ConvertTimeDeliveryToViev(deliverySetting.TimeDelivery);
            }
            else
            {
                ViewBag.DeliveryTimeTable = WeeklyDayHelper.ConvertTimeDeliveryToViev();
                ViewBag.ZoneId = DateTimeHepler.DEFAULT_ZONE_ID;
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

                        upload.SaveAs(Server.MapPath("~/Images/Products/" + newFileName));

                        result.Success = true;
                        result.URL = $"../Images/Products/{newFileName}";
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
        public JsonResult SaveDeliverySetting(DeliverySettingModel setting)
        {
            var branchId = DataWrapper.GetBranchId(User.Identity.Name);
            setting.BranchId = branchId;
            var successSave = DataWrapper.SaveDeliverySetting(setting);
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
            var result = new JsonResultModel();

            if (!System.IO.File.Exists(Server.MapPath(category.Image)))
            {
                category.Image = "/images/default-image.jpg";
            }

            category = DataWrapper.SaveCategory(category);

            if (category != null)
            {
                result.Data = category;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При добавлении категории что то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddProduct(ProductModel product)
        {
            var result = new JsonResultModel();

            if (!System.IO.File.Exists(Server.MapPath(product.Image)))
            {
                product.Image = "/images/default-image.jpg";
            }

            product = DataWrapper.SaveProduct(product);

            if (product != null)
            {
                result.Data = product;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При добавлении продукта что то пошло не так...";
            }

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

        [HttpPost]
        [Authorize]
        public JsonResult LoadCategoryList()
        {
            var result = new JsonResultModel();
            var categories = DataWrapper.GetCategories();

            if (categories != null)
            {
                result.Data = categories;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузки категорий что то пошло не так";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveCategory(int id)
        {
            var result = new JsonResultModel();
            var success = DataWrapper.RemoveCategory(id);

            if (success)
            {
                result.Success = success;
            }
            else
            {
                result.ErrorMessage = "Категория не удалена";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdateCategory(CategoryModel category)
        {
            var result = new JsonResultModel();
            var updateCategory = DataWrapper.UpdateCategory(category);

            if (updateCategory != null)
            {
                result.Data = updateCategory;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "Категория не обновлена";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadProductList(int idCategory)
        {
            var result = new JsonResultModel();
            var products = DataWrapper.GetProducts(idCategory);

            if (products != null)
            {
                result.Data = products;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузки продуктов что то пошло не так";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveProduct(int id)
        {
            var result = new JsonResultModel();
            var success = DataWrapper.RemoveProduct(id);

            if (success)
            {
                result.Success = success;
            }
            else
            {
                result.ErrorMessage = "Продукт не удалена";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdateProduct(ProductModel product)
        {
            var result = new JsonResultModel();
            var updateProduct = DataWrapper.UpdateProduct(product);

            if (updateProduct != null)
            {
                result.Data = updateProduct;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "Продукт не обновлена";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddStock(StockModel stock)
        {
            var result = new JsonResultModel();

            if (!System.IO.File.Exists(Server.MapPath(stock.Image)))
            {
                stock.Image = "/images/default-image.jpg";
            }

            stock = DataWrapper.SaveStock(stock);

            if (stock != null)
            {
                result.Data = stock;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При добавлении акции что то пошло не так...";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult RemoveStock(int id)
        {
            var result = new JsonResultModel();
            var success = DataWrapper.RemoveStock(id);

            if (success)
            {
                result.Success = success;
            }
            else
            {
                result.ErrorMessage = "Акция не удалена";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdateStock(StockModel sotck)
        {
            var result = new JsonResultModel();
            var updateStock = DataWrapper.UpdateStock(sotck);

            if (updateStock != null)
            {
                result.Data = updateStock;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "Акция не обновлена";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LoadStockList()
        {
            var result = new JsonResultModel();
            var stock = DataWrapper.GetStocks();

            if (stock != null)
            {
                result.Data = stock;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "При загрузки акций что то пошло не так";
            }

            return Json(result);
        }
    }
}