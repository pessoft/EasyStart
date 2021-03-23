using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem
{
    public enum IntegrationOrderStatus
    {
        Unknown = 0,
        New = 1,
        Preparing = 2,
        Deliverid = 3,
        Done = 4,
        Canceled = 5,
    }
}