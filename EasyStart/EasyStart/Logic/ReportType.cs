﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public enum ReportType
    {
        Unknown = 0,
        CountOrder = 1,
        Top5Categories = 2,
        Top5Products = 3,
        DeliveryMethod = 4,
        NewUsers = 5,
        Revenue = 6,
        ActiveUsers = 7,
        GeneralUsersQuantity = 8
    }
}