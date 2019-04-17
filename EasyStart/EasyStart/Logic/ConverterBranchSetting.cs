using EasyStart.Models;
using EasyStart.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public class ConverterBranchSetting
    {
        private string templateLoginData = "******";

        public List<BranchSettingViewModel> GetBranchSettingViews(
            List<BranchModel> branches,
            Dictionary<int, SettingModel> settings,
            TypeBranch currentTypeBranch)
        {
            List<BranchSettingViewModel> views = new List<BranchSettingViewModel>();

            foreach (var branch in branches)
            {
                SettingModel setting;
                if (settings.TryGetValue(branch.Id, out setting))
                {
                    var view = GetBranchSettingViews(branch, setting, currentTypeBranch);

                    views.Add(view);
                }
                else
                {
                    continue;
                }
            }

            return views;
        }

        public BranchSettingViewModel GetBranchSettingViews(
           BranchModel branch,
           SettingModel setting,
           TypeBranch currentTypeBranch)
        {
            var view = new BranchSettingViewModel
            {
                Addres = $"Адрес: г.{setting.City}, ул.{setting.Street}, д.{setting.HomeNumber}",
                OperationMode = $"Режим работы: {setting.TimeOpen.ToString("#.00")} - {setting.TimeClose.ToString("#.00")}",
                PhoneNumber = $"Номер телефона: {setting.PhoneNumber}",
                Login = "Логин: " + (currentTypeBranch == TypeBranch.MainBranch ? branch.Login : templateLoginData),
                Password = "Пароль: " + (currentTypeBranch == TypeBranch.MainBranch ? branch.Password : templateLoginData),
            };

            return view;
        }
    }
}