using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class VitualMoneyTransactionModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public VirtualMoneyTransactionType TransactionType { get; set; }
        public double VirtualAmount { get; set; }
        public DateTime Date { get; set; }
    }
}