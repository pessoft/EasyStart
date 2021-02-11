using EasyStart.Models;
using EasyStart.Models.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem
{
    public class Iiko : BaseIntegrationSystem
    {
        public Iiko(IntegrationSystemModel integrationSystemSetting): base(integrationSystemSetting)
        { }

        public override bool SendOrder(IOrderDetails orderDetails)
        {
            throw new NotImplementedException();
        }
    }
}