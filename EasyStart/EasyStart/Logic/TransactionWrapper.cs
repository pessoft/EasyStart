using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public static class TransactionWrapper
    {
        public static void SaveTransaction(VitualMoneyTransactionModel transaction)
        {
            try
            {
                using (var db = new TransactionContext())
                {
                    db.VitualMoneyTransactions.Add(transaction);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }
    }
}