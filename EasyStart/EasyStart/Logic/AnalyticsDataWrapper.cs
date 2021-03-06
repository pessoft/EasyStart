﻿using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public static class AnalyticsDataWrapper
    {
        public static Dictionary<DateTime, double> GetCountOrder(DateTime dateFrom, DateTime dateTo, int branchId)
        {
            var result = new Dictionary<DateTime, double>();

            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Orders
                        .Where(p => p.BranchId == branchId &&
                                    DbFunctions.TruncateTime(p.Date) >= dateFrom &&
                                    DbFunctions.TruncateTime(p.Date) <= dateTo)
                        .GroupBy(p => DbFunctions.TruncateTime(p.Date))
                        .ToDictionary(p => p.Key.Value.Date, p => (double)p.Count());
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static Dictionary<DateTime, double> GetRevenue(DateTime dateFrom, DateTime dateTo, int branchId)
        {
            Dictionary<DateTime, double> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Orders
                        .Where(p => p.OrderStatus == OrderStatus.Processed &&
                                    p.BranchId == branchId &&
                                    DbFunctions.TruncateTime(p.Date) >= dateFrom &&
                                    DbFunctions.TruncateTime(p.Date) <= dateTo)
                        .GroupBy(p => DbFunctions.TruncateTime(p.Date))
                        .ToDictionary(p => p.Key.Value.Date, p => Math.Round(p.Sum(s => s.AmountPayDiscountDelivery), 2));
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static List<OrderModel> GetOrders(DateTime dateFrom, DateTime dateTo, int branchId)
        {
            List<OrderModel> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Orders
                        .Where(p => p.OrderStatus == OrderStatus.Processed &&
                                    p.BranchId == branchId &&
                                    DbFunctions.TruncateTime(p.Date) >= dateFrom &&
                                    DbFunctions.TruncateTime(p.Date) <= dateTo)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static Dictionary<DeliveryType, int> GetDeliveryMethodOrders(DateTime dateFrom, DateTime dateTo, int branchId)
        {
            Dictionary<DeliveryType, int> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Orders
                        .Where(p => p.OrderStatus == OrderStatus.Processed &&
                                    p.BranchId == branchId &&
                                    DbFunctions.TruncateTime(p.Date) >= dateFrom &&
                                    DbFunctions.TruncateTime(p.Date) <= dateTo)
                         .GroupBy(p => p.DeliveryType)
                        .ToDictionary(p => p.Key, p => p.Count());
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static Dictionary<DateTime, double> GetNewUsers(DateTime dateFrom, DateTime dateTo)
        {
            Dictionary<DateTime, double> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Clients
                        .Where(p => DbFunctions.TruncateTime(p.Date) >= dateFrom &&
                                    DbFunctions.TruncateTime(p.Date) <= dateTo)
                         .GroupBy(p => p.Date)
                        .ToDictionary(p => p.Key, p => (double)p.Count());
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static Dictionary<DateTime, List<int>> GetActiveUsers(DateTime dateFrom, DateTime dateTo)
        {
            Dictionary<DateTime, List<int>> result = null;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Orders
                        .Where(p => DbFunctions.TruncateTime(p.Date) >= dateFrom &&
                                    DbFunctions.TruncateTime(p.Date) <= dateTo &&
                                    (p.OrderStatus == OrderStatus.Processed))
                         .GroupBy(p => p.Date)
                        .ToDictionary(
                        p => p.Key,
                        p => p.Select(o => o.ClientId).Distinct().ToList());
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }

            return result;
        }

        public static int GetGeneralUserQuantity(int branchId)
        {
            int result = 0;
            try
            {
                using (var db = new AdminPanelContext())
                {
                    result = db.Clients
                        .Where(p => p.BranchId == branchId)
                        .Count();
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