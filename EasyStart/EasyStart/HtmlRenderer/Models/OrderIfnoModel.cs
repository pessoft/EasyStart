using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.HtmlRenderer.Models
{
    public class OrderIfnoModel
    {
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }

        public string CustomerName { get; set; }
        public string CustomerNumberPhone { get; set; }
        public string OrderDateDelivery { get; set; }
        public string NumberAppliances { get; set; }

        public DeliveryType DeliveryType { get; set; }
        public string DeliveryTypeStr { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HomeNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public string Level { get; set; }
        public string IntercomCode { get; set; }
        public string EntranceNumber { get; set; }

        public string AmountPrice { get; set; }
        public string AmountPayCashBack { get; set; }
        public string DeliveryPrice { get; set; }
        public string Discount { get; set; }
        public string ButType { get; set; }
        public string CashBack { get; set; }
        public string AmountPayDiscountDelivery { get; set; }

        public string Comment { get; set; }

        public List<ProductInfoModel> Products { get; set; }
        public List<ProductInfoModel> BonusProducts { get; set; }
        public List<ConstructorProductInfoModel> ConstructorProducts { get; set; }
    }
}