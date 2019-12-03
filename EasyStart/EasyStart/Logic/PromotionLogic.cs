using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public class PromotionLogic
    {
        public void ProcessingVirtualMoney(int orderId)
        {
            var mainBranch = DataWrapper.GetMainBranch();
            var order = DataWrapper.GetOrder(orderId);
            var client = DataWrapper.GetClient(order.ClientId);

            ProcessingCashback(mainBranch.Id, order, client);
            ProcessingPartners(mainBranch.Id, order, client);
        }

        public void Refund(int orderId)
        {
            var order = DataWrapper.GetOrder(orderId);
            if (order.AmountPayCashBack > 0)
            {
                new TransactionVirtualMoneyLogic(order.Id).AddTransaction(VirtualMoneyTransactionType.Refund, order.AmountPayCashBack);

                var client = DataWrapper.GetClient(order.ClientId);
                client.VirtualMoney += order.AmountPayCashBack;
                DataWrapper.ClientUpdateVirtualMoney(client.Id, client.VirtualMoney);
            }

            if (order.RefferalDiscount > 0)
            {
                DataWrapper.ClientUpdateRefferalDiscount(order.ClientId, order.RefferalDiscount);
            }
        }

        private void ProcessingCashback(int branchId, OrderModel order, Client client)
        {
            var cashbackSetting = DataWrapper.GetPromotionCashbackSetting(branchId);
            if (!cashbackSetting.IsUseCaschback)
                return;

            var cashbackValue = order.AmountPayDiscountDelivery * cashbackSetting.ReturnedValue / 100;
            client.VirtualMoney += cashbackValue;
            DataWrapper.ClientUpdateVirtualMoney(client.Id, client.VirtualMoney);

            new TransactionVirtualMoneyLogic(client.Id).AddTransaction(VirtualMoneyTransactionType.EnrollmentPurchase, cashbackValue);
        }

        private void ProcessingPartners(int branchId, OrderModel order, Client client)
        {
            var partnersSetting = DataWrapper.GetPromotionPartnerSetting(branchId);
            if (!partnersSetting.IsUsePartners)
                return;

            var cashbackValue = order.AmountPayDiscountDelivery * partnersSetting.CashBackRefferalValue / 100;
            var parentRefClient = DataWrapper.GetClient(client.ParentRefferalClientId);

            parentRefClient.VirtualMoney += cashbackValue;
            DataWrapper.ClientUpdateVirtualMoney(parentRefClient.Id, parentRefClient.VirtualMoney);

            new TransactionVirtualMoneyLogic(parentRefClient.Id).AddTransaction(VirtualMoneyTransactionType.EnrollmentRefferal, cashbackValue);
        }
    }
}