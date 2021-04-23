using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using EasyStart.Models.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.Services.IntegrationSystem
{
    public interface IIntegrationSystemLogic
    {
        IntegrationSystemModel Save(IntegrationSystemModel setting);
        IntegrationSystemModel Get(int branchId);
        INewOrderResult SendOrder(
            IOrderDetails orderDetails,
            IIntegrationSystemFactory integrationSystemFactory);
        double GetClientVirtualMoney(
            string phoneNumber,
            int branchId,
            IIntegrationSystemFactory integrationSystemFactory);
    }
}
