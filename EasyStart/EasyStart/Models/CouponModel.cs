using EasyStart.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class CouponModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; }
        public DateTime DateFrom { get;set; }
        public DateTime DateTo { get; set; }
        public string Promocode { get; set; }
        public int Count { get; set; }
        public RewardType RewardType { get; set; }
        public int DiscountValue { get; set; }
        public DiscountType DiscountType { get; set; }
        public int CountBounusProducts { get; set; }
        public string AllowedBounusProductsJSON { get; set; }
        public int CountUsed { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public List<int> AllowedBounusProducts
        {
            get
            {
                if (!string.IsNullOrEmpty(AllowedBounusProductsJSON))
                {
                    return JsonConvert.DeserializeObject<List<int>>(AllowedBounusProductsJSON);
                }

                return null;
            }
        }
    }
}