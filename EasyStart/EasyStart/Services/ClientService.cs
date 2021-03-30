using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
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
        private readonly IDefaultEntityRepository<Client> repository;
        
        public ClientService(IDefaultEntityRepository<Client> repository)
        {
            this.repository = repository;
        }

        public List<Client> GetAll(int branchId)
        {
            return repository.Get(p => p.BranchId == branchId).ToList();
        }

        public Client Get(int id)
        {
            var result = repository.Get(id);

            return result;
        }

        public double SetVirtualMoney(int clientId, double virtualMoney)
        {
            if (virtualMoney < 0)
                throw new Exception("Value virtual money must be greater than zero");

            var client = repository.Get(clientId);
            client.VirtualMoney = virtualMoney;

            repository.Update(client);

            return virtualMoney;
        }

        public void Block(int clientId)
        {
            this.ToggleBlock(clientId);
        }

        public void UnBlock(int clientId)
        {
            this.ToggleBlock(clientId, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">key - phone number, value - cashback </param>
        public void SetVirtualMoney(Dictionary<string, double> data)
        {
            if (data == null || !data.Any())
                return;

            var phones = data.Keys.ToList();
            var clients = repository.Get(p => phones.Contains(p.PhoneNumber)).ToList();
            clients.ForEach(p => p.VirtualMoney = data[p.PhoneNumber]);

            repository.Update(clients);
        }

        private void ToggleBlock(int clientId, bool blocked = true)
        {
            var client = repository.Get(clientId);
            client.Blocked = blocked;

            repository.Update(client);
        }
    }
}