using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using EasyStart.Models.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem
{
    public class Iiko : BaseIntegrationSystem
    {
        public Iiko(IntegrationSystemModel integrationSystemSetting): base(integrationSystemSetting)
        { }

        public override INewOrderResult SendOrder(IOrderDetails orderDetails)
        {
            throw new NotImplementedException();
        }

        public override IntegrationOrderStatus GetIntegrationOrderStatus(int externalOrderStatusId)
        {
            throw new NotImplementedException();
        }

        public override double GetClinetVirtualMoney(string phoneNumber)
        {
            throw new NotImplementedException();
        }
    }
}