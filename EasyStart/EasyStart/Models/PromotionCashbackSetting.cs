using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class PromotionCashbackSetting
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public bool IsUseCaschback { get; set; }
        public int ReturnedValue { get; set; }
        public int PaymentValue { get; set; }
        public DateTime DateSave { get; set; }
    }
}