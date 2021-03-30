using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem.SendNewOrderResult
{
    public class NewOrderResult : INewOrderResult
    {
        public NewOrderResult()
        {}

        public NewOrderResult(string errorMessgae)
        {
            ErrorMessgae = errorMessgae;
        }

        private NewOrderResult(long orderNumber, long externalOrderId)
        {
            Success = true;
            OrderNumber = orderNumber;
            ExternalOrderId = externalOrderId;
        }

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

        public static NewOrderResult GetFakeSuccessResult(long orderNumber, long ExternalOrderId)
        {
            return new NewOrderResult(orderNumber, ExternalOrderId);
        }

        public bool Success { get; }
        public string ErrorMessgae { get; }
        public  long OrderNumber { get; }
        public long ExternalOrderId { get; }
    }
}