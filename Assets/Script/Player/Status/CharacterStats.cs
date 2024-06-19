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
    public int Level { get; private set; } = 1;
    public int StatPoints { get; private set; } = 0;
    public CharacterClass CharacterClass { get; private set; }

    public int Strength { get; private set; } = 0;
    public int Dexterity { get; private set; } = 0;
    public int Vitality { get; private set; } = 0;
    public int Energy { get; private set; } = 0;

    public int Life { get; private set; }
    public int Stamina { get; private set; }
    public int Mana { get; private set; }

    public int Damage { get; private set; }
    public int AttackRating { get; private set; }
    public int Defense { get; private set; }
    public int ChanceToBlock { get; private set; }

    public CharacterStats(CharacterClass characterClass)
    {
        CharacterClass = characterClass;
        InitializeStats();
    }

    private void InitializeStats()
    {
        // 기본 스탯 포인트 할당
        //StatPoints = (Level - 1) * 5;

        // 클래스별 기본 능력치 설정
        switch (CharacterClass)
        {
            case CharacterClass.Barbarian:
                Strength = 30;
                Dexterity = 20;
                Vitality = 25;
                Energy = 10;
                break;
            case CharacterClass.Amazon:
                Strength = 20;
                Dexterity = 25;
                Vitality = 20;
                Energy = 15;
                break;
            case CharacterClass.Paladin:
                Strength = 25;
                Dexterity = 20;
                Vitality = 25;
                Energy = 10;
                break;
        }

        CalculateDerivedStats();
    }

    private void CalculateDerivedStats()
    {
        // 각 능력치에 따른 파생 능력치 계산
        Life = Vitality * 3; // 예시로 설정
        Stamina = Vitality * 2; // 예시로 설정
        Mana = Energy * 2; // 예시로 설정

        // 피해 계산 예시 (무기별로 다르게 적용 필요)
        Damage = Strength;

        // 명중률과 방어력 계산 예시
        AttackRating = Dexterity * 5;
        Defense = Dexterity / 4;

        // 막기 확률 계산 예시
        ChanceToBlock = Dexterity; // 실제 공식을 반영 필요
    }

    public void LevelUp()
    {
        Level++;
        StatPoints += 5;
        CalculateDerivedStats();
    }

    public void InvestStat(string stat)
    {
        if (StatPoints > 0)
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
            StatPoints--;
            CalculateDerivedStats();
        }
    }
}
