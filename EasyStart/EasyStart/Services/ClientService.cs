using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
using EasyStart.Logic.Services.Client;
using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class ClientService
    {
        private readonly IClientLogic clientLogic;
        
        public ClientService(IClientLogic clientLogic)
        {
            this.clientLogic = clientLogic;
        }

        public List<Client> GetAll(int branchId)
        {
            return clientLogic.GetAll(branchId);
        }

        public Client Get(int id)
        {
            return clientLogic.Get(id);
        }

        public double SetVirtualMoney(int clientId, double virtualMoney)
        {
            return clientLogic.SetVirtualMoney(clientId, virtualMoney);
        }

        public void Block(int clientId)
        {
            clientLogic.Block(clientId);
        }

        public void UnBlock(int clientId)
        {
            clientLogic.UnBlock(clientId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">key - phone number, value - cashback </param>
        public void SetVirtualMoney(Dictionary<string, double> data)
        {
            clientLogic.SetVirtualMoney(data);
        }
    }
}