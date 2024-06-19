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
        // �⺻ ���� ����Ʈ �Ҵ�
        //StatPoints = (Level - 1) * 5;

        // Ŭ������ �⺻ �ɷ�ġ ����
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
        // �� �ɷ�ġ�� ���� �Ļ� �ɷ�ġ ���
        Life = Vitality * 3; // ���÷� ����
        Stamina = Vitality * 2; // ���÷� ����
        Mana = Energy * 2; // ���÷� ����

        // ���� ��� ���� (���⺰�� �ٸ��� ���� �ʿ�)
        Damage = Strength;

        // ���߷��� ���� ��� ����
        AttackRating = Dexterity * 5;
        Defense = Dexterity / 4;

        // ���� Ȯ�� ��� ����
        ChanceToBlock = Dexterity; // ���� ������ �ݿ� �ʿ�
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
