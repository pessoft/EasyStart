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
                using (var db = new AdminPanelContext())
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
                using (var db = new AdminPanelContext())
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
                using (var db = new AdminPanelContext())
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
                using (var db = new AdminPanelContext())
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
                using (var db = new AdminPanelContext())
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

        public static List<PartnersTransactionView> GetPartnersTransactions(int clientId)
        {
            List<PartnersTransactionView> result = null;

            try
            {
                using (var db = new AdminPanelContext())
                {
                    var transactions = db.PartnersTransactions.Where(p => p.ClientId == clientId
                     && p.TransactionType == PartnersTransactionType.EnrollmentReferral)                    
                     .ToList();

                    var referralIds = transactions.Select(p => p.ReferralId).ToList();
                    var referallDict = db.Clients.Where(p => referralIds.Contains(p.Id)).ToDictionary(p => p.Id);

                    result = transactions.Select(p => new PartnersTransactionView
                    {
                        Id = p.Id,
                        ClientId = p.ClientId,
                        Date = p.Date,
                        Money = p.Money,
                        ReferralId = p.ReferralId,
                        ReferralPhone = referallDict[p.ReferralId].PhoneNumber,
                        TransactionType = p.TransactionType
                    }).ToList();
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
                using (var db = new AdminPanelContext())
                {
                    result = db.CashbackTransactions.Where(p => p.ClientId == clientId).OrderByDescending(p => p.Date).ToList();
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