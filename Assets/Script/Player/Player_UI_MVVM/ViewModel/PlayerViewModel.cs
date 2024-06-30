using ViewModel;

public class PlayerViewModel : ViewModelBase
{
    private CharacterStats _characterStats;

    public PlayerViewModel(CharacterClass characterClass)
    {
        _characterStats = new CharacterStats { CharacterClass = characterClass };
        InitializeStats();
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
                UpdateDerivedStats();
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
                UpdateDerivedStats();
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
                UpdateDerivedStats();
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
                UpdateDerivedStats();
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
    }

    public void LevelUp()
    {
        _characterStats.Level++;
        _characterStats.StatPoints += 5;
        OnPropertyChanged(nameof(Level));
        OnPropertyChanged(nameof(StatPoints));
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
}
