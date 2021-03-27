using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyStart.Logic;
using EasyStart.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace EasyStart.Hubs
{
    public class OrderHub : Hub
    {
        public static List<BranchSignal> BranchSignals = new List<BranchSignal>();
        private static IHubCallerConnectionContext<dynamic> clients;

        public void AddedNewOrder(OrderModel newOrder)
        {
            var connectionIds = BranchSignals
                .Where(p => p.BranchIds.Contains(newOrder.BranchId))
                .Select(p => p.ConnectionId)
                .Distinct()
                .ToList();

            if (connectionIds != null &&
                connectionIds.Any() &&
                clients != null)
            {
                clients.Clients(connectionIds).addNewOrder(newOrder);
            }
        }

        public void ChangeOrderStatus(int orderId, OrderStatus status, int branchId)
        {
            var connectionIds = BranchSignals
                .Where(p => p.BranchIds.Contains(branchId))
                .Select(p => p.ConnectionId)
                .Distinct()
                .ToList();

            if (connectionIds != null &&
                connectionIds.Any() &&
                clients != null)
            {
                clients.Clients(connectionIds).changeOrderStatus(orderId, (int)status);
            }
        }

        public void Connect(List<int> branchIds)
        {
            if (clients == null)
            {
                clients = Clients;
            }

            var connectionId = Context.ConnectionId;

            if (!BranchSignals.Any(p => p.ConnectionId == connectionId))
            {
                BranchSignals.Add(new BranchSignal { ConnectionId = connectionId, BranchIds = branchIds });
            }
            else
            {
                var branchSignal = BranchSignals.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);

                if (branchSignal != null)
                {
                    branchSignal.BranchIds = branchIds;
                }
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = BranchSignals.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                BranchSignals.Remove(item);
                var id = Context.ConnectionId;
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}