using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Models;
using EasyStart.Models.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem
{
    public class WithoutIntegration : BaseIntegrationSystem
    {
        public WithoutIntegration(IntegrationSystemModel integrationSystemSetting) : base(integrationSystemSetting)
        {}

        public override double GetClinetVirtualMoney(string phoneNumber, double defaultValue)
        {
            return defaultValue;
        }

        public override IntegrationOrderStatus GetIntegrationOrderStatus(int externalOrderStatusId)
        {
            return IntegrationOrderStatus.Unknown;
        }

        public override INewOrderResult SendOrder(IOrderDetails orderDetails)
        {
            return new NewOrderResult();
        }
    }
}