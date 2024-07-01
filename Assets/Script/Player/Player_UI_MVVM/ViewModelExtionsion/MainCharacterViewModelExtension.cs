using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViewModel.Extensions
{
    public static class MainCharacterViewModelExtension
    {
        public static void RefreshViewModel(this MainCharacterProfileViewModel vm)
        {
            UIManager.Instance.RefreshCharacterInfo(vm.OnRefreshViewModel);
        }

        public static void OnRefreshViewModel(this MainCharacterProfileViewModel vm, string userId, string name, int level)
        {
            vm.UserId = userId;
            vm.Name = name;
            vm.Level = level;
        }

        public static void RegisterEventsOnEnable(this MainCharacterProfileViewModel vm)
        {
            //GameLogicManager.Inst.RegisterLevelUpCallback(vm.OnResponseLevelUp);
        }

        public static void UnRegisterOnDisable(this MainCharacterProfileViewModel vm)
        {
            //GameLogicManager.Inst.UnRegisterLevelUpCallback(vm.OnResponseLevelUp);
        }

        public static void OnResponseLevelUp(this MainCharacterProfileViewModel vm, int userId, int level)
        {
            //if (vm.UserId != userId)
            //    return;
            //
            //vm.Level = level;
        }
    }
}
