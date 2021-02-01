using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem
{
    public abstract class BaseIntegrationSystem : IIntegrationSystem
    {
        private readonly IntegrationSystemModel integrationSystemSetting;

        public BaseIntegrationSystem(IntegrationSystemModel integrationSystemSetting)
        {
            this.integrationSystemSetting = integrationSystemSetting;
        }

        public abstract bool SendOrder(OrderModel order);
    }
}