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
    }
}