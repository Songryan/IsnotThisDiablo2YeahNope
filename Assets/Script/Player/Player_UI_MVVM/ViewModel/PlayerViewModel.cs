using System;
using ViewModel;

public class PlayerViewModel : ViewModelBase
{
    private CharacterStats _characterStats;

    public PlayerViewModel(CharacterClass characterClass)
    {
        _characterStats = new CharacterStats { CharacterClass = characterClass };
        InitializeStats();
    }

    public string UserId
    {
        get => _characterStats.UserId;
        set
        {
            if (_characterStats.UserId != value)
            {
                _characterStats.UserId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }
    }

    public string Name
    {
        get => _characterStats.Name;
        set
        {
            if (_characterStats.Name != value)
            {
                _characterStats.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public int Level
    {
        get => _characterStats.Level;
        set
        {
            if (_characterStats.Level != value)
            {
                _characterStats.Level = value;
                OnPropertyChanged(nameof(Level));
                OnPropertyChanged(nameof(LevelUpExp)); // Level이 변경될 때 LevelUpExp도 변경됨
                CalculateLevelUpExp();
            }
        }
    }

    public int StatPoints
    {
        get => _characterStats.StatPoints;
        set
        {
            if (_characterStats.StatPoints != value)
            {
                _characterStats.StatPoints = value;
                OnPropertyChanged(nameof(StatPoints));
            }
        }
    }

    public int CurrentExp
    {
        get => _characterStats.CurrentExp;
        set
        {
            if (_characterStats.CurrentExp != value)
            {
                _characterStats.CurrentExp = value;
                OnPropertyChanged(nameof(CurrentExp));
            }
        }
    }

    public int LevelUpExp => _characterStats.LevelUpExp;

    public CharacterClass CharacterClass => _characterStats.CharacterClass;

    public int Strength
    {
        get => _characterStats.Strength;
        set
        {
            if (_characterStats.Strength != value)
            {
                _characterStats.Strength = value;
                OnPropertyChanged(nameof(Strength));
                CalculateDerivedStats();
            }
        }
    }

    public int Dexterity
    {
        get => _characterStats.Dexterity;
        set
        {
            if (_characterStats.Dexterity != value)
            {
                _characterStats.Dexterity = value;
                OnPropertyChanged(nameof(Dexterity));
                CalculateDerivedStats();
            }
        }
    }

    public int Vitality
    {
        get => _characterStats.Vitality;
        set
        {
            if (_characterStats.Vitality != value)
            {
                _characterStats.Vitality = value;
                OnPropertyChanged(nameof(Vitality));
                CalculateDerivedStats();
            }
        }
    }

    public int Energy
    {
        get => _characterStats.Energy;
        set
        {
            if (_characterStats.Energy != value)
            {
                _characterStats.Energy = value;
                OnPropertyChanged(nameof(Energy));
                CalculateDerivedStats();
            }
        }
    }

    public int Life => _characterStats.Life;
    public int Stamina => _characterStats.Stamina;
    public int Mana => _characterStats.Mana;
    public int Damage => _characterStats.Damage;
    public int AttackRating => _characterStats.AttackRating;
    public int Defense => _characterStats.Defense;
    public int ChanceToBlock => _characterStats.ChanceToBlock;

    private void UpdateDerivedStats()
    {
        OnPropertyChanged(nameof(Life));
        OnPropertyChanged(nameof(Stamina));
        OnPropertyChanged(nameof(Mana));
        OnPropertyChanged(nameof(Damage));
        OnPropertyChanged(nameof(AttackRating));
        OnPropertyChanged(nameof(Defense));
        OnPropertyChanged(nameof(ChanceToBlock));
    }

    private void InitializeStats()
    {
        switch (_characterStats.CharacterClass)
        {
            case CharacterClass.Barbarian:
                _characterStats.Strength = 30;
                _characterStats.Dexterity = 20;
                _characterStats.Vitality = 25;
                _characterStats.Energy = 10;
                break;
            case CharacterClass.Amazon:
                _characterStats.Strength = 20;
                _characterStats.Dexterity = 25;
                _characterStats.Vitality = 20;
                _characterStats.Energy = 15;
                break;
            case CharacterClass.Paladin:
                _characterStats.Strength = 25;
                _characterStats.Dexterity = 20;
                _characterStats.Vitality = 25;
                _characterStats.Energy = 10;
                break;
        }
        CalculateDerivedStats();
    }

    public void LevelUp()
    {
        _characterStats.Level++;
        _characterStats.StatPoints += 5;
        OnPropertyChanged(nameof(Level));
        OnPropertyChanged(nameof(StatPoints));
        OnPropertyChanged(nameof(LevelUpExp)); // LevelUpExp도 변경됨
    }

    public void InvestStat(string stat)
    {
        if (_characterStats.StatPoints > 0)
        {
            switch (stat.ToLower())
            {
                case "strength":
                    Strength++;
                    break;
                case "dexterity":
                    Dexterity++;
                    break;
                case "vitality":
                    Vitality++;
                    break;
                case "energy":
                    Energy++;
                    break;
            }
            _characterStats.StatPoints--;
            OnPropertyChanged(nameof(StatPoints));
        }
    }

    private void CalculateDerivedStats()
    {
        _characterStats.Life = _characterStats.Vitality * 3;
        _characterStats.Stamina = _characterStats.Vitality * 2;
        _characterStats.Mana = _characterStats.Energy * 2;
        _characterStats.Damage = _characterStats.Strength;
        _characterStats.AttackRating = _characterStats.Dexterity * 5;
        _characterStats.Defense = _characterStats.Dexterity / 4;
        _characterStats.ChanceToBlock = _characterStats.Dexterity;
        UpdateDerivedStats();
    }

    private void CalculateLevelUpExp()
    {
        if (_characterStats.Level == 1)
        {
            _characterStats.LevelUpExp = 100;
        }
        else
        {
            _characterStats.LevelUpExp = (int)(100 * Math.Pow(1.5, _characterStats.Level - 1));
        }
        OnPropertyChanged(nameof(LevelUpExp));
    }
}
