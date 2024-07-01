using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIData;

namespace ViewModel.Extensions
{ 
    public static class StatPopupVIewModelExtension
    {
        public static void RefreshViewModel(this StatPopupViewModel vm)
        {
            JsonDataManager.Instance.RefreshCharacterInfo(vm.OnRefreshViewModel);
        }

        public static void OnRefreshViewModel(this StatPopupViewModel vm, string userId, string name, int level)
        {
            vm.CharacterName = name;
            vm.CharacterLevel = level;
            vm.Strength = JsonDataManager.Instance.CharIntProp[$"{userId}_Strength"];
            vm.Dexterity = JsonDataManager.Instance.CharIntProp[$"{userId}_Dexterity"];
            vm.Vitality = JsonDataManager.Instance.CharIntProp[$"{userId}_Vitality"];
            vm.Energy = JsonDataManager.Instance.CharIntProp[$"{userId}_Energy"];
            vm.Stamina = JsonDataManager.Instance.CharIntProp[$"{userId}_Stamina"];
            vm.Life = JsonDataManager.Instance.CharIntProp[$"{userId}_Life"];
            vm.Mana = JsonDataManager.Instance.CharIntProp[$"{userId}_Mana"];
            vm.Skill1 = JsonDataManager.Instance.CharIntProp[$"{userId}_Damage"];
            vm.Skill2 = JsonDataManager.Instance.CharIntProp[$"{userId}_Damage"];
            vm.AttackRating = JsonDataManager.Instance.CharIntProp[$"{userId}_AttackRating"];
            vm.Defense = JsonDataManager.Instance.CharIntProp[$"{userId}_Defense"];
            vm.ChanceToBlock = JsonDataManager.Instance.CharIntProp[$"{userId}_ChanceToBlock"];
            vm.NewStatPoint = JsonDataManager.Instance.CharIntProp[$"{userId}_StatPoints"];
        }
    }
}
