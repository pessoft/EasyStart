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

            if (order.ReferralDiscount > 0)
            {
                DataWrapper.ClientUpdateReferralDiscount(order.ClientId, order.ReferralDiscount);
            }

            if (order.CouponId > 0)
            {
                RefunCopunCountUser(order.CouponId);
            }
        }

        public void UseCopun(int couponId)
        {
            var errMessage = "Купон не действителен";
            if (couponId > 0)
            {
                var coupon = DataWrapper.GetCoupon(couponId);
                ++coupon.CountUsed;

                if (coupon.CountUsed >= coupon.Count)
                    throw new Exception(errMessage);

                DataWrapper.UpdateCouponCountUser(couponId, coupon.CountUsed);
            }
        }

        private void RefunCopunCountUser(int couponId)
        {
            if (couponId > 0)
            {
                var coupon = DataWrapper.GetCoupon(couponId);
                --coupon.CountUsed;

                if (coupon.CountUsed >= 0)
                    DataWrapper.UpdateCouponCountUser(couponId, coupon.CountUsed);
            }
        }

        public List<StockModel> GetStockForAPI(int branchId, int clientId)
        {
            var stocks = GetStock(branchId, clientId);
            stocks.ForEach(p => PreprocessorDataAPI.ChangeImagePath(p));

            return stocks;
        }

        public List<StockModel> GetStock(int branchId, int clientId)
        {
            List<StockModel> stocks = DataWrapper.GetActiveStocks(branchId); ;
            var oneOffStockIds = stocks
                .Where(p => p.StockTypePeriod == StockTypePeriod.OneOff)
                .Select(p => p.Id)
                .ToList();

            if (oneOffStockIds != null && oneOffStockIds.Any())
            {
                var usedOneOffStockIds = DataWrapper.GetUsedOneOffStockIds(clientId, oneOffStockIds);

                if (usedOneOffStockIds != null && usedOneOffStockIds.Any())
                {
                    stocks = stocks
                        .Where(p => !usedOneOffStockIds.Contains(p.Id))
                        .ToList();
                }
            }

            return stocks;
        }

        public List<CouponModel> GetCoupons(int branchId)
        {
            List<CouponModel> coupons = DataWrapper.GetActiveCoupons(branchId);

            return coupons;
        }

        public PromotionCashbackSetting GetSettingCashBack(int branchId)
        {
            return DataWrapper.GetPromotionCashbackSetting(branchId);
        }

        public List<PromotionSectionSetting> GetSettingSections(int branchId)
        {
            return DataWrapper.GetPromotionSettings(branchId);
        }

        public PromotionPartnerSetting GetSettingPartners(int branchId)
        {
            return DataWrapper.GetPromotionPartnerSetting(branchId);
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

            var cashbackValue = order.AmountPayDiscountDelivery * partnersSetting.CashBackReferralValue / 100;
            var parentRefClient = DataWrapper.GetClient(client.ParentReferralClientId);

            parentRefClient.VirtualMoney += cashbackValue;
            DataWrapper.ClientUpdateVirtualMoney(parentRefClient.Id, parentRefClient.VirtualMoney);

            new TransactionVirtualMoneyLogic(parentRefClient.Id).AddTransaction(VirtualMoneyTransactionType.EnrollmentReferral, cashbackValue);
        }
    }
}