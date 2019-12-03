using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class TransactionContext : DbContext
    {
        public DbSet<VitualMoneyTransactionModel> VitualMoneyTransactions { get; set; }
    }
}