using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public enum StockConditionTriggerType
    {
        Unknown = 0,
        DeliveryOrder = 1,
        SummOrder = 2,
        ProductsOrder = 3,
        HappyBirthday = 4
    }
}