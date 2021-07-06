using EasyStart.Logic.Services.RepositoryFactory;
using EasyStart.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using ClientModel = EasyStart.Models.Client;

namespace EasyStart.Logic.Services.Client
{
    public class ClientLogic: IClientLogic
    {
        private readonly IRepository<ClientModel, int> repository;

        public ClientLogic(IRepositoryFactory repositoryFactory)
        {
            repository = repositoryFactory.GetRepository<ClientModel, int>();
        }

        public List<ClientModel> GetAll(int branchId)
        {
            return repository.Get(p => p.BranchId == branchId).ToList();
        }

        public ClientModel Get(int id)
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

        /// <summary>
        /// Тут ни чего не удаляется, а просто у клиентов сбрасываются настройки филиала
        /// </summary>
        /// <param name="branchId"></param>
        public void RemoveByBranch(int branchId)
        {
            var clients = repository.Get(p => p.BranchId == branchId).ToList();
            clients.ForEach(p => { p.BranchId = -1; p.CityId = -1; });

            repository.Update(clients);
        }
    }
}