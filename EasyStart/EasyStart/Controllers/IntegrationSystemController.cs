using EasyStart.Logic;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Models;
using EasyStart.Models.Integration;
using EasyStart.Repository;
using EasyStart.Services;
using Ganss.Excel;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models.ProductOption;
using EasyStart.Logic.Services.Order;
using EasyStart.Logic.Services.IntegrationSystem;
using EasyStart.Logic.Services.Product;
using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.Client;
using EasyStart.Logic.Services.DeliverySetting;
using EasyStart.Logic.Services.PushNotification;
using EasyStart.Logic.Services.GeneralSettings;
using EasyStart.Logic.Services;
using EasyStart.Logic.Services.CategoryProduct;
using EasyStart.Logic.Services.RepositoryFactory;

namespace EasyStart.Controllers
{
    public class IntegrationSystemController : Controller
    {
        private readonly IntegrationSystemService integrationSystemService;
        private readonly BranchService branchService;
        private readonly OrderService orderService;
        private readonly ProductService productService;
        private readonly ClientService clientService;

        public IntegrationSystemController()
        {
            var context = new AdminPanelContext();
            var repositoryFactory = new RepositoryFactory(context);
            var imageLogic = new ContainImageLogic();
            var displayItemSettingLogic = new DisplayItemSettingLogic();

            var orderLogic = new OrderLogic(repositoryFactory);
            var integrationSystemLogic = new IntegrationSystemLogic(repositoryFactory);
            var productLogic = new ProductLogic(
                repositoryFactory,
                imageLogic,
                displayItemSettingLogic);
            var deliverySettingLogic = new DeliverySettingLogic(repositoryFactory);
            var pushNotificationLogic = new PushNotificationLogic(repositoryFactory);
            var branchLogic = new BranchLogic(repositoryFactory);
            var clientLogic = new ClientLogic(repositoryFactory);
            var generalSettingLogic = new GeneralSettingsLogic(repositoryFactory);
            var categoryProductLogic = new CategoryProductLogic(
                repositoryFactory,
                imageLogic,
                displayItemSettingLogic);

            orderService = new OrderService(
                orderLogic,
                integrationSystemLogic,
                productLogic,
                deliverySettingLogic,
                pushNotificationLogic);
            integrationSystemService = new IntegrationSystemService(integrationSystemLogic);
            clientService = new ClientService(clientLogic);
            branchService = new BranchService(branchLogic, generalSettingLogic);
            productService = new ProductService(productLogic, branchLogic, categoryProductLogic);
        }

        [HttpGet]
        [Authorize]
        public JsonResult Get()
        {
            var result = new JsonResultModel();

            try
            {
                var branch = branchService.Get(User.Identity.Name);
                var setting = integrationSystemService.Get(branch.Id);

                result.Success = true;
                result.Data = setting;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При получении натсройки интеграционной системы что-то пошло не так.";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Save(IntegrationSystemModel setting)
        {
            var result = new JsonResultModel();

            try
            {
                var branch = branchService.Get(User.Identity.Name);
                setting.BranchId = branch.Id;

                var savedSetting = integrationSystemService.Save(setting);
                if (savedSetting == null)
                    throw new Exception("Настройка интеграционной системы не сохранена");

                result.Data = savedSetting;
                result.Success = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При сохранении натсройки интеграционной системы что-то пошло не так.";
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SendOrder(int orderId)
        {
            var result = new JsonResultModel();

            try
            {
                var newOrderResult = orderService.SendOrderToIntegrationSystem(orderId);

                result.Success = newOrderResult.Success;
                result.Data = newOrderResult.OrderNumber;
                result.ErrorMessage = newOrderResult.ErrorMessgae;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SyncFrontpadClientsCashback()
        {
            var result = new JsonResultModel();
            string uploadedFilePath = null;
            try
            {
                foreach (string file in Request.Files)
                {
                    var upload = Request.Files[file];
                    if (upload != null)
                    {
                        string fileName = System.IO.Path.GetFileName(upload.FileName);
                        string ext = fileName.Substring(fileName.LastIndexOf("."));

                        if (ext != ".xlsx")
                        {
                            result.Success = false;
                            result.ErrorMessage = "Не поддерживаемый формат файла. Необходим файл с расширение *.xlsx";

                            return Json(result);
                        }

                        fileName = String.Format(@"{0}{1}", System.Guid.NewGuid(), ext);
                        uploadedFilePath = Server.MapPath("~/Tmp/" + fileName);

                        upload.SaveAs(uploadedFilePath);


                        var clients = new ExcelMapper(uploadedFilePath)
                            .Fetch<IntegrationClient>()
                            .Select(p =>
                                {
                                    var phoneChars = p.PhoneNumber.Where(x => Char.IsDigit(x)).ToArray();
                                    var virualMoney = 0.0;
                                    var phoneNumber = "";

                                    if (phoneChars.Length == 11)
                                    {
                                        phoneChars[0] = '7';
                                        var phoneHowNumber = long.Parse(new string(phoneChars));
                                        phoneNumber = phoneHowNumber.ToString("+#(###)###-##-##");
                                        virualMoney = p.VirtualMoney;
                                    }

                                    return new IntegrationClient { PhoneNumber = phoneNumber, VirtualMoney = virualMoney };
                                }
                            )
                            .Where(p => !string.IsNullOrEmpty(p.PhoneNumber))
                            .GroupBy(p => p.PhoneNumber)
                            .ToDictionary(p => p.Key, p => p.Max(x => x.VirtualMoney));

                        clientService.SetVirtualMoney(clients);

                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.ErrorMessage = "Не удалось загрузить файл";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
                result.Success = false;
                result.ErrorMessage = "При синхронизации что-то пошло не так";
            }
            finally
            {
                DeleteTmpFile(uploadedFilePath);
            }

            return Json(result);
        }

        private void DeleteTmpFile(string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
    }
}