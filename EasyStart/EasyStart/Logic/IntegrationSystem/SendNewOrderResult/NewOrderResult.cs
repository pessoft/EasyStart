using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem.SendNewOrderResult
{
    public class NewOrderResult : INewOrderResult
    {
        public NewOrderResult(FrontPadNewOrderResult result)
        {
            Success = result.Success;
            ErrorMessgae = result.ErrorMessage;
            OrderNumber = result.OrderNumber;
            ExternalOrderId = result.OrderId;
        }

        public NewOrderResult(IikoNewOrderResult result)
        {
            throw new Exception("Constructor not implemented");
        }

        
        public bool Success { get; }
        public string ErrorMessgae { get; }
        public  long OrderNumber { get; }
        public long ExternalOrderId { get; }
    }
}