using EasyStart.Models;
using EasyStart.Models.Transaction;
using System;
using System.Linq;

namespace EasyStart.Logic.Transaction
{
    public static class TransactionWrapper
    {
        public static void AddCashbackTransaction(CashbackTransaction transaction)
        {
            if (transaction == null)
                return;

            try
            {
                using (var db = new TransactionContext())
                {
                    db.CashbackTransactions.Add(transaction);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static void AddRefundCashbackTransactiion(RefundCashbackTransactionModel transaction)
        {
            if (transaction == null)
                return;

            try
            {
                using (var db = new TransactionContext())
                {
                    db.RefundCashbackTransactions.Add(transaction);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static void AddPartnersTransaction(PartnersTransaction transaction)
        {
            if (transaction == null)
                return;

            try
            {
                using (var db = new TransactionContext())
                {
                    db.PartnersTransactions.Add(transaction);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        public static CashbackTransaction GetCashbackTransaction(int orderId)
        {
            CashbackTransaction result = null;

            if (orderId < 1)
                return result;

            try
            {
                using (var db = new TransactionContext())
                {
                    result =  db.CashbackTransactions.FirstOrDefault(p => p.OrderId == orderId);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result; 
        }
    }
}