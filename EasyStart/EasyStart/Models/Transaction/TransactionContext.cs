using System.Data.Entity;

namespace EasyStart.Models.Transaction
{
    public class TransactionContext : DbContext
    {
        public DbSet<CashbackTransaction> CashbackTransactions { get; set; }
        public DbSet<RefundCashbackTransactionModel> RefundCashbackTransactions { get; set; }
        public DbSet<PartnersTransaction> PartnersTransactions { get; set; }
    }
}