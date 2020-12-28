using EasyStart.Models;
using EasyStart.Repositories;
using EasyStart.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyStart.Controllers
{
    public class IntegrationSystemController: Controller
    {
        private readonly IntegrationSystemService integrationSystemService;
        private readonly BranchService branchService;

        public IntegrationSystemController()
        {
            var context = new AdminPanelContext();

            var inegrationSystemRepository = new InegrationSystemRepository(context);
            integrationSystemService = new IntegrationSystemService(inegrationSystemRepository);

            var branchRepository = new BranchRepository(context);
            branchService = new BranchService(branchRepository);
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

                result.Success = true;
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex);
                result.ErrorMessage = "При сохранении натсройки интеграционной системы что-то пошло не так.";
            }
            
            return Json(result);
        }
    }
}