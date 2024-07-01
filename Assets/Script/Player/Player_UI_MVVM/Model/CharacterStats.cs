using System;
using System.Collections.Generic;
using UnityEngine;

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

[Serializable]
public class CharacterStats
{
    public string UserId;
    public string Name;
    public int Strength;
    public int Dexterity;
    public int Vitality;
    public int Energy;
    public int Level;
    public int StatPoints;
    public string CharacterClassString;

    [NonSerialized]
    public CharacterClass CharacterClass;

    public int CurrentExp;
    public int LevelUpExp;

    public int Life { get; set; }
    public int Stamina { get; set; }
    public int Mana { get; set; }
    public int Damage { get; set; }
    public int AttackRating { get; set; }
    public int Defense { get; set; }
    public int ChanceToBlock { get; set; }

    public void ConvertStringToEnum()
    {
        if (Enum.TryParse(CharacterClassString, out CharacterClass characterClass))
        {
            CharacterClass = characterClass;
        }
        else
        {
            Debug.LogWarning($"Invalid CharacterClass string: {CharacterClassString}");
        }
    }

    public void ConvertEnumToString()
    {
        CharacterClassString = CharacterClass.ToString();
    }
}

[Serializable]
public class CharacterData
{
    public List<CharacterStats> Characters;
}
