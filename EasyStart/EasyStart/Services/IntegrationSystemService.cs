using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using EasyStart.Models.Integration;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EasyStart.Services
{
    public class IntegrationSystemService
    {
        private readonly IDefaultEntityRepository<IntegrationSystemModel> repository;
        
        public IntegrationSystemService(IDefaultEntityRepository<IntegrationSystemModel> repository)
        {
            this.repository = repository;
        }

        public IntegrationSystemModel Save(IntegrationSystemModel setting) 
        {
            IntegrationSystemModel result;

            var oldSetting = setting.Id > 0 ? null : Get(setting.BranchId);
            setting.Id = oldSetting?.Id ?? setting.Id;

            if (setting.Id > 0)
            {
                setting.Id = oldSetting.Id;

                repository.Update(setting);
                result = repository.Get(setting.Id);
            }
            else
            {
                result = repository.Create(setting);
            }

            return result;
        }

        public IntegrationSystemModel Get(int branchId)
        {
            var result = repository.Get(p => p.BranchId == branchId).FirstOrDefault();

            return result;
        }

        public INewOrderResult SendOrder(
            IOrderDetails orderDetails,
            IIntegrationSystemFactory integrationSystemFactory)
        {
            var order = orderDetails.GetOrder();
            var integrationSystemSetting = Get(order.BranchId);
            var integrationSystem = integrationSystemFactory.GetIntegrationSystem(integrationSystemSetting);
            var result = integrationSystem.SendOrder(orderDetails);

            return result;
        }

        public double GetClientVirtualMoney(
            string phoneNumber,
            int branchId,
            IIntegrationSystemFactory integrationSystemFactory,
            double defaultValue = 0)
        {
            var integrationSystemSetting = Get(branchId);
            var integrationSystem = integrationSystemFactory.GetIntegrationSystem(integrationSystemSetting);
            var result = integrationSystem.GetClinetVirtualMoney(phoneNumber, defaultValue);

            return result;
        }
    }
}