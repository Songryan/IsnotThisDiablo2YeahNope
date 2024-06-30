using System;

public enum CharacterClass
{
    Barbarian,
    Amazon,
    Assassin,
    Necromancer,
    Druid,
    Sorceress,
    Paladin
}

public class CharacterStats
{
    private int _strength;
    private int _dexterity;
    private int _vitality;
    private int _energy;

    public int Level { get; set; } = 1;
    public int StatPoints { get; set; } = 0;
    public CharacterClass CharacterClass { get; set; }

    public int Strength
    {
        get => _strength;
        set
        {
            _strength = value;
            CalculateDerivedStats();
        }
    }

    public int Dexterity
    {
        get => _dexterity;
        set
        {
            _dexterity = value;
            CalculateDerivedStats();
        }
    }

    public int Vitality
    {
        get => _vitality;
        set
        {
            _vitality = value;
            CalculateDerivedStats();
        }
    }

    public int Energy
    {
        get => _energy;
        set
        {
            _energy = value;
            CalculateDerivedStats();
        }
    }

    public int Life { get; private set; }
    public int Stamina { get; private set; }
    public int Mana { get; private set; }

    public int Damage { get; private set; }
    public int AttackRating { get; private set; }
    public int Defense { get; private set; }
    public int ChanceToBlock { get; private set; }

    private void CalculateDerivedStats()
    {
        Life = Vitality * 3;
        Stamina = Vitality * 2;
        Mana = Energy * 2;
        Damage = Strength;
        AttackRating = Dexterity * 5;
        Defense = Dexterity / 4;
        ChanceToBlock = Dexterity;
    }
}
