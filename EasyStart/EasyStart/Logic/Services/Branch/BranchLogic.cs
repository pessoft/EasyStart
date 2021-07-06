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
        private readonly IRepository<BranchModel, int> repository;
        private readonly string userLogin;

        public BranchLogic(IRepository<BranchModel, int> repository, string userLogin)
        {
            this.repository = repository;
            this.userLogin = userLogin;
        }

        public BranchLogic(IRepository<BranchModel, int> repository) : this(repository, null)
        {}

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

        public BranchModel Get()
        {
            if (string.IsNullOrEmpty(this.userLogin))
                throw new Exception("User login must not be null or empty");

            return Get(this.userLogin);
        }

        public BranchModel GetMainBranch()
        {
            return repository.Get(p => p.TypeBranch == Logic.TypeBranch.MainBranch).FirstOrDefault();
        }

        public void RemoveByBranch(int branchId)
        {
            var branch = repository.Get(branchId);
            branch.IsDeleted = true;

            repository.Update(branch);
        }

        public List<BranchSettingViewModel> GetBranches(IEnumerable<SettingModel> generalSettings)
        {
            var branch = Get();
            var converter = new ConverterBranchSetting();
            var branchViewvs = converter.GetBranchSettingViews(
                repository.Get().ToList(),
                generalSettings.ToDictionary(p => p.BranchId),
                branch.TypeBranch);

            return branchViewvs;
        }
    }
}