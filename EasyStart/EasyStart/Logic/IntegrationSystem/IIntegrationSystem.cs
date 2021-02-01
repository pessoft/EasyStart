﻿using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.IntegrationSystem
{
    public interface IIntegrationSystem
    {
        bool SendOrder(OrderModel order);
    }
}
