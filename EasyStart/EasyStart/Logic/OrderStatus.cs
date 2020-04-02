using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public enum OrderStatus
    {
        Processing = 0,
        Processed = 1,
        Cancellation = 2,
        Deleted = 3,
        PendingPay = 4,
    }
}