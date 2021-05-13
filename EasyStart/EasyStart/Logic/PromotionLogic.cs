using EasyStart.Logic.Transaction;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public class PromotionLogic
    {
        private TransactionLogic transactionLogic;

        public PromotionLogic()
        {
            transactionLogic = new TransactionLogic();
        }

        public void ProcessingVirtualMoney(int orderId, int branchId)
        {
            var order = DataWrapper.GetOrder(orderId);
            var client = DataWrapper.GetClient(order.ClientId);

            ProcessingCashback(branchId, order, client);
            ProcessingPartners(branchId, order, client);
        }

        public void Refund(int orderId)
        {
            var order = DataWrapper.GetOrder(orderId);
            if (order.AmountPayCashBack > 0)
            {

                var client = DataWrapper.GetClient(order.ClientId);
                client.VirtualMoney += order.AmountPayCashBack;
                DataWrapper.ClientUpdateVirtualMoney(client.Id, client.VirtualMoney);

                var cashbackTransaction = TransactionWrapper.GetCashbackTransaction(orderId);
                transactionLogic.AddRefundCashbackTransaction(cashbackTransaction.Id);
            }

            if (order.ReferralDiscount > 0)
            {
                DataWrapper.ClientUpdateReferralDiscount(order.ClientId, order.ReferralDiscount);
            }

            if (order.CouponId > 0)
            {
                RefundCopunCountUser(order.CouponId);
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

        private void RefundCopunCountUser(int couponId)
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

        public List<PromotionNewsModel> GetNewsForAPI(int branchId)
        {
            var news = DataWrapper.GetPromotionNews(branchId);
            news.ForEach(p => PreprocessorDataAPI.ChangeImagePath(p));

            return news;
        }

        public List<int> GetStockIdsWithTriggerBirthdayForExcluded(List<StockModel> stocks, int clientId)
        {
            var excluded = new List<int>();

            if (stocks == null || !stocks.Any())
                return excluded;

            var stocksWithTriggerBirhday = stocks
                .Where(p => p.ConditionType == StockConditionTriggerType.HappyBirthday)
                .ToList();

            if (stocksWithTriggerBirhday.Any())
            {
                var client = DataWrapper.GetClient(clientId);

                if (client.DateBirth != null)
                {
                    var isAllowUseStockWithBirhda = DataWrapper.AllowUseStockWithBirthday(client.Id);

                    if (isAllowUseStockWithBirhda)
                    {
                        stocksWithTriggerBirhday.ForEach(p =>
                        {
                            var dateBefore = DateTime.Now;
                            dateBefore = dateBefore.AddDays(-p.ConditionBirthdayBefore);


                            var dateAfter = DateTime.Now;
                            dateAfter = dateAfter.AddDays(p.ConditionBirthdayAfter);

                            var dateBirth = new DateTime(dateAfter.Year, client.DateBirth.Value.Month, client.DateBirth.Value.Day);

                            if (!(dateBefore.Date <= dateBirth.Date &&
                            dateBirth.Date <= dateAfter.Date))
                                excluded.Add(p.Id);
                        });
                    }
                    else
                    {
                        excluded = stocksWithTriggerBirhday.Select(p => p.Id).ToList();
                    }
                }
                else
                {
                    excluded = stocksWithTriggerBirhday.Select(p => p.Id).ToList();
                }
            }

            return excluded;
        }

        public List<StockModel> GetStock(int branchId, int clientId)
        {
            List<StockModel> stocks = DataWrapper.GetActiveStocks(branchId);

            if (clientId < 1)
            {
                return stocks;
            }

            Action<StockModel, List<int>> onlyShowSetter = (p, blackList) =>
            {
                if (!p.OnlyShowNews)
                    p.OnlyShowNews = blackList.Contains(p.Id);
            };

            var excludedStocksWithBirthday = GetStockIdsWithTriggerBirthdayForExcluded(stocks, clientId);
            stocks.ForEach(p => onlyShowSetter(p, excludedStocksWithBirthday));

            List<StockModel> stocksOneOff = stocks
                .Where(p => p.StockTypePeriod == StockTypePeriod.OneOff)
                .ToList();
            var oneOrderStockGuids = stocksOneOff
                .Where(p => p.StockOneTypeSubtype == StockOneTypeSubtype.OneOrder)
                .Select(p => p.UniqId)
                .ToList();
            var guidDictOneOff = oneOrderStockGuids == null || !oneOrderStockGuids.Any() ? null : DataWrapper.GetStockIdsByGuid(oneOrderStockGuids);

            var oneOrderStockIds = guidDictOneOff?.SelectMany(p => p.Value).ToList();
            if (oneOrderStockIds != null && oneOrderStockIds.Any())
            {
                var usedOneOffIds = DataWrapper.GetUsedOneOffStockIds(clientId, oneOrderStockIds);

                var usedOneOffStockIds = new List<int>();

                foreach (var kv in guidDictOneOff)
                {
                    if (kv.Value.Exists(p => usedOneOffIds.Contains(p)))
                        usedOneOffStockIds.AddRange(kv.Value);
                }

                if (usedOneOffStockIds.Any())
                {
                    stocks.ForEach(p => onlyShowSetter(p, usedOneOffStockIds));
                }
            }

            var firstStockGuids = stocksOneOff
                .Where(p => p.StockOneTypeSubtype == StockOneTypeSubtype.FirstOrder)
                .Select(p => p.UniqId)
                .ToList();

            var guidDictFirstOrder = firstStockGuids == null || !firstStockGuids.Any() ? null : DataWrapper.GetStockIdsByGuid(firstStockGuids);
            var ferstStockIds = guidDictFirstOrder?.SelectMany(p => p.Value).ToList();

            if (ferstStockIds != null && ferstStockIds.Any() && !DataWrapper.IsEmptyOrders(clientId))
            {
                stocks.ForEach(p => onlyShowSetter(p, ferstStockIds));
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
            return DataWrapper.GetPromotionSectionSettings(branchId);
        }

        public PromotionPartnerSetting GetSettingPartners(int branchId)
        {
            return DataWrapper.GetPromotionPartnerSetting(branchId);
        }

        private void ProcessingCashback(int branchId, OrderModel order, Client client)
        {
            var cashbackSetting = DataWrapper.GetPromotionCashbackSetting(branchId);
            if (!cashbackSetting.IsUseCashback || (order.AmountPayCashBack != 0 && cashbackSetting.AlwaysApplyCashback == false))
                return;

            var cashbackValue = order.AmountPayDiscountDelivery * cashbackSetting.ReturnedValue / 100;
            client.VirtualMoney += cashbackValue;
            DataWrapper.ClientUpdateVirtualMoney(client.Id, client.VirtualMoney);

            transactionLogic.AddCashbackTransaction(CashbackTransactionType.EnrollmentPurchase, client.Id, order.Id, cashbackValue);
        }

        private void ProcessingPartners(int branchId, OrderModel order, Client client)
        {
            var partnersSetting = DataWrapper.GetPromotionPartnerSetting(branchId);
            if (!partnersSetting.IsUsePartners || client.ParentReferralClientId < 1)
                return;

            var parentRefClient = DataWrapper.GetClient(client.ParentReferralClientId);
            if (partnersSetting.IsCashBackReferralOnce
                && transactionLogic.ContainsTransaction(PartnersTransactionType.EnrollmentReferral, parentRefClient.Id, client.Id))
                return;

            var cashbackValue = order.AmountPayDiscountDelivery * partnersSetting.CashBackReferralValue / 100;

            parentRefClient.VirtualMoney += cashbackValue;
            DataWrapper.ClientUpdateVirtualMoney(parentRefClient.Id, parentRefClient.VirtualMoney);

            transactionLogic.AddPartnersTransaction(PartnersTransactionType.EnrollmentReferral, parentRefClient.Id, client.Id, cashbackValue);
        }
    }
}