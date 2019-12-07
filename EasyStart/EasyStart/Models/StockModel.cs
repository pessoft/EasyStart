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
    public class StockModel : IContainImage
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public StockTypePeriod StockTypePeriod { get; set; }
        public StockOneTypeSubtype StockOneTypeSubtype { get; set; }
        public DateTime StockFromDate { get; set; }
        public DateTime StockToDate { get; set; }
        public RewardType RewardType { get; set; }
        public int DiscountValue { get; set; }
        public DiscountType DiscountType { get; set; }
        public int CountBonusProducts { get; set; }

        public string AllowedBonusProductsJSON { get; set; }

        [NotMapped]
        public List<int> AllowedBonusProducts
        {
            get
            {
                if (!string.IsNullOrEmpty(AllowedBonusProductsJSON))
                {
                    return JsonConvert.DeserializeObject<List<int>>(AllowedBonusProductsJSON);
                }

                return null;
            }
        }
        public StockConditionTriggerType ConditionType { get; set; }
        public StockConditionDeliveryType ConditionDeliveryType { get; set; }
        public int ConditionOrderSum { get; set; }

        public string ConditionCountProductsJSON { get; set; }

        [NotMapped]
        [ScriptIgnore]
        public Dictionary<int, int> ConditionCountProducts
        {
            get
            {
                if (!string.IsNullOrEmpty(ConditionCountProductsJSON))
                {
                    return JsonConvert.DeserializeObject<Dictionary<int, int>>(ConditionCountProductsJSON);
                }

                return null;
            }
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
    }
}