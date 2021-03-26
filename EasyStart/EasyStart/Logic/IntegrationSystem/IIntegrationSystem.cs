using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using EasyStart.Models.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.IntegrationSystem
{
    public interface IIntegrationSystem
    {
        INewOrderResult SendOrder(IOrderDetails orderDetails, string domainUrl);
        double GetClinetVirtualMoney(string phoneNumber);
        IntegrationOrderStatus GetIntegrationOrderStatus(int externalOrderStatusId);
    }
}
