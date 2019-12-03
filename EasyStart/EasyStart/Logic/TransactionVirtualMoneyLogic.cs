using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public class TransactionVirtualMoneyLogic
    {
        private int clientId;
        public TransactionVirtualMoneyLogic(int clientId)
        {
            this.clientId = clientId;
        }

        public void AddTransaction(VirtualMoneyTransactionType transactionType, double virtualAmount)
        {
            if (transactionType == VirtualMoneyTransactionType.OrderPayment)
            {
                virtualAmount = -Math.Abs(virtualAmount);
            }

            var transaction = new VitualMoneyTransactionModel
            {
                ClientId = clientId,
                Date = DateTime.Now,
                TransactionType = transactionType,
                VirtualAmount = virtualAmount
            };

            SaveTransaction(transaction);
        }

        private void SaveTransaction(VitualMoneyTransactionModel transaction)
        {
            TransactionWrapper.SaveTransaction(transaction);
        }
    }
}