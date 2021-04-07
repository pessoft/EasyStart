using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Models.Integration
{
    public interface IOrderDetails
    {
        OrderModel GetOrder();
        ProductModel GetProduct(int id);
        string GetAreaDeliveryCode();
        AdditionalFilling GetAdditionalFilling(int id);
        AdditionOptionItem GetAdditionOptionItem(int id);
    }
}
