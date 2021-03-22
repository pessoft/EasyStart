using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem
{
    public enum IntegrationOrderStatus
    {
        Unknown,
        New,
        Preparing,
        Deliverid,
        Done,
        Canceled,
    }
}