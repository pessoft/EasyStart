﻿using EasyStart.Logic.IntegrationSystem.SendNewOrderResult;
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
        private readonly IRepository<Client> repository;
        
        public ClientService(IRepository<Client> repository)
        {
            this.repository = repository;
        }

        public List<Client> Get()
        {
            return repository.Get().ToList();
        }

        public Client Get(int id)
        {
            var result = repository.Get(id);

            return result;
        }

        public void Block(int clientId)
        {
            this.ToggleBlock(clientId);
        }

        public void UnBlock(int clientId)
        {
            this.ToggleBlock(clientId, false);
        }

        private void ToggleBlock(int clientId, bool blocked = true)
        {
            var client = repository.Get(clientId);
            client.Blocked = blocked;

            repository.Update(client);
        }
    }
}