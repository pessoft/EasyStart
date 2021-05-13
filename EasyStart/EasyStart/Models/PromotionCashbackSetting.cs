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
        public bool IsUseCashback { get; set; }
        public int ReturnedValue { get; set; }
        public int PaymentValue { get; set; }
        public DateTime DateSave { get; set; }
        /// <summary>
        /// Если false, то кешбек не начисляется если заказ частично оплачен кешбеком.
        /// Если true, то кешбек всегда начисляется, даже если заказ частично оплачен кешбеком.
        /// </summary>
        public bool AlwaysApplyCashback { get; set; }
    }
}