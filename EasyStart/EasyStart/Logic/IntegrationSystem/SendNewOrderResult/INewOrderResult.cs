using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.IntegrationSystem.SendNewOrderResult
{
    public interface INewOrderResult
    {
        bool Success { get; }
        string ErrorMessgae { get; }
        int OrderNumber { get; }
    }
}
