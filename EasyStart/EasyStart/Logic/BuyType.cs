using EasyStart.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public enum BuyType
    {
        [Description("Наличные")]
        Cash =1,
        [Description("Банковская карта")]
        Card =2,
        [Description("Онлайн")]
        Online = 3,
    }
}