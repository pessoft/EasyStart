using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class IntegrationSystemService
    {
        private readonly IRepository<IntegrationSystemModal> repository;
        
        public IntegrationSystemService(IRepository<IntegrationSystemModal> repository)
        {
            this.repository = repository;
        }

        public IntegrationSystemModal Save(IntegrationSystemModal setting) 
        {
            IntegrationSystemModal result;

            if (setting.Id > 0)
            {
                repository.Update(setting);
                result = repository.Get(setting.Id);
            }
            else
            {
                result = repository.Create(setting);
            }

            return result;
        }

        public IntegrationSystemModal Get(int branchId)
        {
            var result = repository.Get(p => p.BranchId == branchId).FirstOrDefault();

            return result;
        }
    }
}