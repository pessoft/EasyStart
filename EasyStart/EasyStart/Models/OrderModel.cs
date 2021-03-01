using EasyStart.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace EasyStart.Models
{
    public class OrderModel: BaseEntity
    {
        public int BranchId { get; set; }
        public int CityId { get; set; }
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public string Street { get; set; }
        public string HomeNumber { get; set; }
        public string EntranceNumber { get; set; }
        public string ApartamentNumber { get; set; }
        public string Level { get; set; }
        public string IntercomCode { get; set; }
        public BuyType BuyType { get; set; }
        public string Comment { get; set; }
        public string ProductCountJSON { get; set; }
        
        public double DiscountPercent { get; set; }
        public double DiscountRuble { get; set; }

        /// <summary>
        /// Одноразовая скидка полученная от реферала
        /// </summary>
        public double ReferralDiscount { get; set; }

        public double DeliveryPrice { get; set; }
        /// <summary>
        /// Сдача с суммы
        /// </summary>
        public double CashBack { get; set; }
        /// <summary>
        /// Сумма к оплате
        /// </summary>
        public double AmountPay { get; set; }
        /// <summary>
        /// Сумма к оплате с учетом скидки, доставки и кешбека
        /// </summary>
        public double AmountPayDiscountDelivery { get; set; }
        public bool NeedCashBack { get; set; }
        public DateTime Date { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? DateDelivery { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Processing;

        public string ProductBonusCountJSON { get; set; }

        /// <summary>
        /// Оплачено кешбеком
        /// </summary>
        public double AmountPayCashBack { get; set; }

        public int NumberAppliances { get; set; }

        [NotMapped]
        public List<int> StockIds { get; set; }

        public int CouponId { get; set; }

        public string CommentCauseCancel { get; set; } 

        public long IntegrationOrderNumber { get; set; }

        public bool IsSendToIntegrationSystem { get; set; }

        [NotMapped]
        [ScriptIgnore]
        public Dictionary<int, int> ProductCount
        {
            get
            {
                if (!string.IsNullOrEmpty(ProductCountJSON))
                    return JsonConvert.DeserializeObject<Dictionary<int, int>>(ProductCountJSON);
                else
                    return null;

            }
        }

        [NotMapped]
        [ScriptIgnore]
        public Dictionary<int, int> ProductBonusCount
        {
            get
            {
                if (!string.IsNullOrEmpty(ProductBonusCountJSON))
                    return JsonConvert.DeserializeObject<Dictionary<int, int>>(ProductBonusCountJSON);
                else
                    return null;
            }
        }

        public string ProductConstructorCountJSON { get; set; }

        [NotMapped]
        [ScriptIgnore]
        public List<ProductConstructorOrderModel> ProductConstructorCount
        {
            get
            {
                if (!string.IsNullOrEmpty(ProductConstructorCountJSON))
                    return JsonConvert.DeserializeObject<List<ProductConstructorOrderModel>>(ProductConstructorCountJSON);
                else
                    return null;
            }
        }

        public string ProductWithOptionsCountJSON { get; set; }

        [NotMapped]
        [ScriptIgnore]
        public List<ProductWithOptionsOrderModel> ProductWithOptionsCount
        {
            get
            {
                if (!string.IsNullOrEmpty(ProductWithOptionsCountJSON))
                    return JsonConvert.DeserializeObject<List<ProductWithOptionsOrderModel>>(ProductWithOptionsCountJSON);
                else
                    return null;
            }
        }
    }
}