using EasyStart.Models;
using EasyStart.Models.Transaction;
using System;
using System.Collections.Generic;
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

        public static bool ContainsTransaction(PartnersTransactionType transactionType, int parentClientId, int referralId)
        {
            bool result = false;

            try
            {
                using (var db = new TransactionContext())
                {
                    var search = db.PartnersTransactions.Where(p => p.TransactionType == transactionType
                    && p.ClientId == parentClientId
                    && p.ReferralId == referralId)
                    .Take(1)
                    .ToList();

                    if (search != null && search.Any())
                        result = search.Count > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<PartnersTransaction> GetPartnersTransactions(int clientId)
        {
            List<PartnersTransaction> result = null;

            try
            {
                using (var db = new TransactionContext())
                {
                    result = db.PartnersTransactions.Where(p => p.ClientId == clientId).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<CashbackTransaction> GetCashbackTransactions(int clientId)
        {
            List<CashbackTransaction> result = null;

            try
            {
                using (var db = new TransactionContext())
                {
                    result = db.CashbackTransactions.Where(p => p.ClientId == clientId).ToList();
                    if (result != null && result.Any())
                    {
                        var ids = result.Select(p => p.Id).ToList();

                        var refundTransactaionIds = db.RefundCashbackTransactions
                            .Where(p => ids.Contains(p.CashbackTransactionId))
                            .Select(p => p.CashbackTransactionId)
                            .ToList();

                        if (refundTransactaionIds != null && refundTransactaionIds.Any())
                        {
                            result = result
                                .Where(p => !refundTransactaionIds.Contains(p.Id))
                                .ToList();
                        }
                    }
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