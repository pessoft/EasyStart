using EasyStart.Models.ProductOption;
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
        private readonly Dictionary<int, AdditionalFilling> additionalFillings;
        private readonly Dictionary<int, AdditionOptionItem> additionOptionItems;

        public OrderDetails(
            OrderModel order,
            List<ProductModel> products,
            List<AdditionalFilling> additionalFillings,
            List<AdditionOptionItem> additionOptionItems,
            List<AreaDeliveryModel> areaDeliveries)
        {
            this.order = order ?? throw new Exception("Order must not be null");
            this.products = products != null && products.Any() ?
                products.ToDictionary(p => p.Id) :
                throw new Exception("Products must not be empty");

            this.additionalFillings = additionalFillings != null ?
                additionalFillings.ToDictionary(p => p.Id) :
                null;

            this.additionOptionItems = additionOptionItems != null ?
                additionOptionItems.ToDictionary(p => p.Id) :
                null;

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

        public AdditionalFilling GetAdditionalFilling(int id)
        {
            if (additionalFillings != null
                && additionalFillings.TryGetValue(id, out AdditionalFilling additionalFilling))
                return additionalFilling;

            throw new Exception($"AdditionalFilling with id = {id} not found");
        }

        public ProductModel GetProduct(int id)
        {
            if (products.TryGetValue(id, out ProductModel product))
                return product;

            throw new Exception($"Product with id = {id} not found");
        }

        public AdditionOptionItem GetAdditionOptionItem(int id)
        {
            if (additionOptionItems != null
                && additionOptionItems.TryGetValue(id, out AdditionOptionItem additionOptionItem))
                return additionOptionItem;

            throw new Exception($"AdditionOptionItem with id = {id} not found");
        }
    }
}