using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.Transaction
{
    public class RefundCashbackTransactionModel
    {
        public int Id { get; set; }
        public int CashbackTransactionId { get; set; }
        public DateTime Date { get; set; }
    }
}