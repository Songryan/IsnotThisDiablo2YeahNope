using System.Collections;
using System.Collections.Generic;
using ViewModel;

public class StatPopupViewModel : ViewModelBase
{
    private string _characterName;
    private int _characterLevel;
    private int _characterExp;
    private int _characterCurrentExp;

    private int _strength;
    private int _dexterity;
    private int _vitality;
    private int _energy;

    private int _skill1;
    private int _skill2;

    private int _attackRating;
    private int _chanceToBlock;
    private int _defense;

    private int _stamina;
    private int _life;
    private int _mana;

    private int _newStatPoint;

    public string CharacterName
    {
        get => _characterName;
        set
        {
            if (_characterName != value)
            {
                _characterName = value;
                OnPropertyChanged(nameof(CharacterName));
            }
        }
    }

    public int CharacterLevel
    {
        get => _characterLevel;
        set
        {
            if (_characterLevel != value)
            {
                _characterLevel = value;
                OnPropertyChanged(nameof(CharacterLevel));
            }
        }
    }

    public int CharacterExp
    {
        get => _characterExp;
        set
        {
            if (_characterExp != value)
            {
                _characterExp = value;
                OnPropertyChanged(nameof(CharacterExp));
            }
        }
    }

    public int CharacterCurrentExp
    {
        get => _characterCurrentExp;
        set
        {
            if (_characterCurrentExp != value)
            {
                _characterCurrentExp = value;
                OnPropertyChanged(nameof(CharacterCurrentExp));
            }
        }
    }

    public int Strength
    {
        get => _strength;
        set
        {
            if (_strength != value)
            {
                _strength = value;
                OnPropertyChanged(nameof(Strength));
            }
        }
    }

    public int Dexterity
    {
        get => _dexterity;
        set
        {
            if (_dexterity != value)
            {
                _dexterity = value;
                OnPropertyChanged(nameof(Dexterity));
            }
        }
    }

    public int Vitality
    {
        get => _vitality;
        set
        {
            if (_vitality != value)
            {
                _vitality = value;
                OnPropertyChanged(nameof(Vitality));
            }
        }
    }

    public int Energy
    {
        get => _energy;
        set
        {
            if (_energy != value)
            {
                _energy = value;
                OnPropertyChanged(nameof(Energy));
            }
        }
    }

    public int Skill1
    {
        get => _skill1;
        set
        {
            if (_skill1 != value)
            {
                _skill1 = value;
                OnPropertyChanged(nameof(Skill1));
            }
        }
    }

    public int Skill2
    {
        get => _skill2;
        set
        {
            if (_skill2 != value)
            {
                _skill2 = value;
                OnPropertyChanged(nameof(Skill2));
            }
        }
    }

    public int AttackRating
    {
        get => _attackRating;
        set
        {
            if (_attackRating != value)
            {
                _attackRating = value;
                OnPropertyChanged(nameof(AttackRating));
            }
        }
    }

    public int ChanceToBlock
    {
        get => _chanceToBlock;
        set
        {
            if (_chanceToBlock != value)
            {
                _chanceToBlock = value;
                OnPropertyChanged(nameof(ChanceToBlock));
            }
        }
    }

    public int Defense
    {
        get => _defense;
        set
        {
            if (_defense != value)
            {
                _defense = value;
                OnPropertyChanged(nameof(Defense));
            }
        }
    }

    public int Stamina
    {
        get => _stamina;
        set
        {
            if (_stamina != value)
            {
                _stamina = value;
                OnPropertyChanged(nameof(Stamina));
            }
        }
    }

    public int Life
    {
        get => _life;
        set
        {
            if (_life != value)
            {
                _life = value;
                OnPropertyChanged(nameof(Life));
            }
        }
    }

    public int Mana
    {
        get => _mana;
        set
        {
            if (_mana != value)
            {
                _mana = value;
                OnPropertyChanged(nameof(Mana));
            }
        }
    }

    public int NewStatPoint
    {
        get => _newStatPoint;
        set
        {
            if (_newStatPoint != value)
            {
                _newStatPoint = value;
                OnPropertyChanged(nameof(NewStatPoint));
            }
        }
    }
}
