﻿using EasyStart.Models;
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

        public override bool SendOrder(OrderModel order)
        {
            throw new NotImplementedException();
        }
    }
}