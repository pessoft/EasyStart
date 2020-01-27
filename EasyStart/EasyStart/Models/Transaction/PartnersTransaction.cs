using EasyStart.Logic.Transaction;
using System;

namespace EasyStart.Models.Transaction
{
    public class PartnersTransaction
    {
        public int Id { get; set; }
        public PartnersTransactionType TransactionType { get; set; }
        public int ClientId { get; set; }
        public int ReferralId { get; set; }
        public double Money { get; set; }
        public DateTime Date { get; set; }
    }
}