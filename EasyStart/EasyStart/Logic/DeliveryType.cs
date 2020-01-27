using EasyStart.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public enum DeliveryType
    {
        [Description("Самовывоз")]
        TakeYourSelf = 1,

        [Description("Доставка курьером")]
        Delivery = 2
    }
}