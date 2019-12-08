using EasyStart.Logic;
using EasyStart.Logic.Transaction;
using System;

namespace EasyStart.Models.Transaction
{
    public class CashbackTransaction
    {
        public int Id { get; set; }
        public CashbackTransactionType TransactionType { get; set; }
        public int ClientId { get; set; }
        public int OrderId { get; set; }
        public double Money { get; set; }
        public DateTime Date { get; set; }
    }
}