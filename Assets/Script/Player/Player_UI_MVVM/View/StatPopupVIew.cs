using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Extensions;

public class StatPopupView : MonoBehaviour
{
    [SerializeField] Text Text_CharacterName;
    [SerializeField] Text Text_CharacterLevel;
    [SerializeField] Text Text_CharacterExp;
    [SerializeField] Text Text_CharacterCurrentExp;

    [SerializeField] Text Text_Str;
    [SerializeField] Text Text_Dex;
    [SerializeField] Text Text_Vital;
    [SerializeField] Text Text_Energy;

    [SerializeField] Text Text_Skill1;
    [SerializeField] Text Text_Skill2;

    [SerializeField] Text Text_AtkRating;
    [SerializeField] Text Text_ChanceToBlock;
    [SerializeField] Text Text_Defense;

    [SerializeField] Text Text_Stamina;
    [SerializeField] Text Text_Life;
    [SerializeField] Text Text_Mana;

    [SerializeField] Text Text_NewStatPoint;

    private StatPopupViewModel _vm;

    private void OnEnable()
    {
        if (_vm == null)
        {
            _vm = new StatPopupViewModel();
            _vm.PropertyChanged += OnPropertyChanged;
            _vm.RefreshViewModel();  // 데이터 로드 및 초기화
        }
    }

    private void OnDisable()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropertyChanged;
            _vm = null;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_vm.CharacterName):
                Text_CharacterName.text = _vm.CharacterName;
                break;
            case nameof(_vm.CharacterLevel):
                Text_CharacterLevel.text = $"{_vm.CharacterLevel}";
                break;
            case nameof(_vm.CharacterExp):
                Text_CharacterExp.text = $"{_vm.CharacterExp}";
                break;
            case nameof(_vm.CurrentExp):
                Text_CharacterCurrentExp.text = $"{_vm.CurrentExp}";
                break;
            case nameof(_vm.Strength):
                Text_Str.text = $"{_vm.Strength}";
                break;
            case nameof(_vm.Dexterity):
                Text_Dex.text = $"{_vm.Dexterity}";
                break;
            case nameof(_vm.Vitality):
                Text_Vital.text = $"{_vm.Vitality}";
                break;
            case nameof(_vm.Energy):
                Text_Energy.text = $"{_vm.Energy}";
                break;
            case nameof(_vm.Skill1):
                Text_Skill1.text = $"{_vm.Skill1}";
                break;
            case nameof(_vm.Skill2):
                Text_Skill2.text = $"{_vm.Skill2}";
                break;
            case nameof(_vm.AttackRating):
                Text_AtkRating.text = $"{_vm.AttackRating}";
                break;
            case nameof(_vm.ChanceToBlock):
                Text_ChanceToBlock.text = $"{_vm.ChanceToBlock}";
                break;
            case nameof(_vm.Defense):
                Text_Defense.text = $"{_vm.Defense}";
                break;
            case nameof(_vm.Stamina):
                Text_Stamina.text = $"{_vm.Stamina}";
                break;
            case nameof(_vm.Life):
                Text_Life.text = $"{_vm.Life}";
                break;
            case nameof(_vm.Mana):
                Text_Mana.text = $"{_vm.Mana}";
                break;
            case nameof(_vm.NewStatPoint):
                Text_NewStatPoint.text = $"{_vm.NewStatPoint}";
                break;
        }
    }
}
