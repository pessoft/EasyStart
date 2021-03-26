using EasyStart.Logic;
using EasyStart.Logic.IntegrationSystem;
using EasyStart.Models;
using EasyStart.Models.Integration;
using EasyStart.Repositories;
using EasyStart.Services;
using Ganss.Excel;
using Microsoft.Office.Interop.Excel;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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

            var inegrationSystemRepository = new InegrationSystemRepository(context);
            integrationSystemService = new IntegrationSystemService(inegrationSystemRepository);

            var branchRepository = new BranchRepository(context);
            branchService = new BranchService(branchRepository);

            var orderRepository = new OrderRepository(context);
            orderService = new OrderService(orderRepository);

            var productRepository = new ProductRepository(context);
            productService = new ProductService(productRepository);

            var clientRepository = new ClientRepository(context);
            clientService = new ClientService(clientRepository);
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
                var order = orderService.Get(orderId);
                var products = productService.Get(order);
                var orderDetails = new OrderDetails(order, products);
                var domainUrl = Request.Url.GetBaseUrl();
                var newOrderResult = integrationSystemService.SendOrder(
                    orderDetails,
                    new IntegrationSystemFactory(),
                    domainUrl);

                orderService.MarkOrderSendToIntegrationSystem(orderId, newOrderResult);

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
            string clientsSavedPath = Server.MapPath($"~/Tmp/{System.Guid.NewGuid()}.xlsx"); ;
            try
            {
                foreach (string file in Request.Files)
                {
                    var upload = Request.Files[file];
                    if (upload != null)
                    {
                        string fileName = System.IO.Path.GetFileName(upload.FileName);
                        string ext = fileName.Substring(fileName.LastIndexOf("."));
                        fileName = String.Format(@"{0}{1}", System.Guid.NewGuid(), ext);
                        uploadedFilePath = Server.MapPath("~/Tmp/" + fileName);
                        
                        upload.SaveAs(uploadedFilePath);

                        Application app = new Application();
                        app.Visible = false;
                        Workbook wb = app.Workbooks.Open(uploadedFilePath);
                        wb.SaveAs(clientsSavedPath, XlFileFormat.xlOpenXMLWorkbook);
                        wb.Close();
                        app.Quit();


                        var clients = new ExcelMapper(clientsSavedPath)
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
                DeleteTmpFile(clientsSavedPath);
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