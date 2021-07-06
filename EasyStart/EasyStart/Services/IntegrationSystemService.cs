using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Logic.Services.IntegrationSystem;
using EasyStart.Models;
using EasyStart.Models.Integration;
using EasyStart.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EasyStart.Services
{
    public class IntegrationSystemService
    {
        private readonly IIntegrationSystemLogic integrationSystemLogic;
        
        public IntegrationSystemService(IIntegrationSystemLogic integrationSystemLogic)
        {
            this.integrationSystemLogic = integrationSystemLogic;
        }

        public IntegrationSystemModel Save(IntegrationSystemModel setting)
        {
            return integrationSystemLogic.Save(setting);
        }

        public IntegrationSystemModel Get(int branchId)
        {
            return integrationSystemLogic.Get(branchId);
        }

        public INewOrderResult SendOrder(
            IOrderDetails orderDetails,
            IIntegrationSystemFactory integrationSystemFactory)
        {
            return integrationSystemLogic.SendOrder(orderDetails, integrationSystemFactory);
        }

        public double GetClientVirtualMoney(
            string phoneNumber,
            int branchId,
            IIntegrationSystemFactory integrationSystemFactory)
        {
            return integrationSystemLogic.GetClientVirtualMoney(phoneNumber, branchId, integrationSystemFactory);
        }
    }
}