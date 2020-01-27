using EasyStart.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Transaction
{
    public class TransactionLogic
    {
        public void AddCashbackTransaction(CashbackTransactionType transactionType, int clientId, int orderId, double money)
        {
            var transaction = new CashbackTransaction
            {
                TransactionType = transactionType,
                ClientId = clientId,
                OrderId = orderId,
                Money = transactionType == CashbackTransactionType.OrderPayment ? -Math.Abs(money) : money,
                Date = DateTime.Now
            };

            TransactionWrapper.AddCashbackTransaction(transaction);
        }

        public void AddRefundCashbackTransaction(int cashbackTransactionId)
        {
            var transaction = new RefundCashbackTransactionModel
            {
                CashbackTransactionId = cashbackTransactionId,
                Date = DateTime.Now
            };

            TransactionWrapper.AddRefundCashbackTransactiion(transaction);
        }

        public void AddPartnersTransaction(PartnersTransactionType transactionType, int clientId, int referralId, double money)
        {
            var transaction = new PartnersTransaction
            {
                TransactionType = transactionType,
                ClientId = clientId,
                ReferralId = referralId,
                Money = money,
                Date = DateTime.Now
            };

            TransactionWrapper.AddPartnersTransaction(transaction);
        }

        public void AddPartnersTransaction(PartnersTransactionType transactionType, int clientId, double money)
        {
            AddPartnersTransaction(transactionType, clientId, 0, money);
        }

        public bool ContainsTransaction(PartnersTransactionType transactionType, int parentClientId, int referralId)
        {
            return TransactionWrapper.ContainsTransaction(transactionType, parentClientId, referralId);
        }
    }
}