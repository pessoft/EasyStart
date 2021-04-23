using EasyStart.Logic.Services.Branch;
using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class BranchService
    {
        private readonly IBranchLogic branchLogic;

        public BranchService(IBranchLogic branchLogic)
        {
            this.branchLogic = branchLogic;
        }

        public BranchModel Save(BranchModel branch)
        {
            return branchLogic.Save(branch);
        }

        public BranchModel Get(string login) 
        {
            return branchLogic.Get(login);
        }

        public BranchModel GetMainBranch()
        {
            return branchLogic.GetMainBranch();
        }
    }
}