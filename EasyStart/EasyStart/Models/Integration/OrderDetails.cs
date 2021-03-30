using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.Integration
{
    public class OrderDetails : IOrderDetails
    {
        private readonly OrderModel order;
        private readonly Dictionary<int, ProductModel> products;
        private readonly string areaDeliveryVendorCode;

        public OrderDetails(
            OrderModel order,
            List<ProductModel> products,
            List<AreaDeliveryModel> areaDeliveries)
        {
            this.order = order ?? throw new Exception("Order must not be null");
            this.products = products != null && products.Any() ?
                products.ToDictionary(p => p.Id) :
                throw new Exception("Products must not be empty");

            if (string.IsNullOrEmpty(order.AreaDeliveryId))
                areaDeliveryVendorCode = null;
            else
                areaDeliveryVendorCode = areaDeliveries.FirstOrDefault(p => p.UniqId == order.AreaDeliveryId)?.VendorCode;
        }

        public string GetAreaDeliveryCode()
        {
            return areaDeliveryVendorCode;
        }

        public OrderModel GetOrder()
        {
            return order;
        }

        public ProductModel GetProduct(int id)
        {
            if (products.TryGetValue(id, out ProductModel product))
                return product;

            throw new Exception($"Product with id = {id} not found");
        }
    }
}