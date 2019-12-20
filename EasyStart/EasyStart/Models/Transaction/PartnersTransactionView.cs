using EasyStart.Logic.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.Transaction
{
    public class PartnersTransactionView
    {
        public int Id { get; set; }
        public PartnersTransactionType TransactionType { get; set; }
        public int ClientId { get; set; }
        public int ReferralId { get; set; }
        public  string ReferralPhone { get; set; }
        public double Money { get; set; }
        public DateTime Date { get; set; }
    }
}