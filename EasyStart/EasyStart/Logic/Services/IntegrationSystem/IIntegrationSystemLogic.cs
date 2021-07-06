using EasyStart.Logic.IntegrationSystem;
using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using EasyStart.Models.Integration;

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
