using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Branch
{
    public class BranchLogic : IBranchLogic
    {
        private readonly IDefaultEntityRepository<BranchModel> repository;

        public BranchLogic(IDefaultEntityRepository<BranchModel> repository)
        {
            this.repository = repository;
        }

        public BranchModel Save(BranchModel branch)
        {
            BranchModel result;

            if (branch.Id > 0)
            {
                repository.Update(branch);
                result = repository.Get(branch.Id);
            }
            else
            {
                result = repository.Create(branch);
            }

            return result;
        }

        public BranchModel Get(string login)
        {
            return repository.Get(p => p.Login == login).FirstOrDefault();
        }

        public BranchModel GetMainBranch()
        {
            return repository.Get(p => p.TypeBranch == Logic.TypeBranch.MainBranch).FirstOrDefault();
        }
    }
}