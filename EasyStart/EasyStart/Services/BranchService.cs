using EasyStart.Logic;
using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.GeneralSettings;
using EasyStart.Models;
using EasyStart.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class BranchService
    {
        private readonly IBranchLogic branchLogic;
        private readonly IGeneralSettingsLogic generalSettingsLogic;

        public BranchService(
            IBranchLogic branchLogic,
            IGeneralSettingsLogic generalSettingsLogic)
        {
            this.branchLogic = branchLogic;
            this.generalSettingsLogic = generalSettingsLogic;
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

        public BranchSettingViewModel AddBranch(NewBranchModel newBranch)
        {
            var currentBranch = branchLogic.Get();
            var mainBranch = branchLogic.GetMainBranch();
            var newBranchAlreadyExist = branchLogic.Get(newBranch.Login) != null;

            if (currentBranch.Id != mainBranch.Id)
                throw new BranchActionPermissionDeniedException();

            if (newBranchAlreadyExist)
                throw new BranchAlreadyExistException();

            var branch = new BranchModel
            {
                Login = newBranch.Login,
                Password = newBranch.Password,
                TypeBranch = TypeBranch.SubBranch
            };


            var savedBranch = branchLogic.Save(branch);

            var setting = new SettingModel
            {
                BranchId = savedBranch.Id,
                CityId = newBranch.CityId
            };

            generalSettingsLogic.SaveSetting(setting);

            var baseBrachClone = new BranchClone(mainBranch.Id, savedBranch.Id);
            baseBrachClone.Clone();

            var converter = new ConverterBranchSetting();
            var branchView = converter.GetBranchSettingViews(branch, setting, TypeBranch.MainBranch);

            new PromotionDefaultSetting(savedBranch.Id).SaveSettings();

            return branchView;
        }

        public List<BranchSettingViewModel> GetBranches()
        {
            var settings = generalSettingsLogic.Get();
            var branchViews = branchLogic.GetBranches(settings);

            return branchViews;
        }
    }
}