using EasyStart.Models;
using EasyStart.Models.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem
{
    public abstract class BaseIntegrationSystem : IIntegrationSystem
    {
        protected readonly IntegrationSystemModel integrationSystemSetting;

        public BaseIntegrationSystem(IntegrationSystemModel integrationSystemSetting)
        {
            this.integrationSystemSetting = integrationSystemSetting;
        }

        public abstract bool SendOrder(IOrderDetails orderDetails);
    }
}