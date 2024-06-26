using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GameDataEntry
{
    public int GlobalID;
    public int Level;
    public int Quality;
    public string StatBonus;

    public GameDataEntry(int globalID, int level, int quality, string statBonus)
    {
        GlobalID = globalID;
        Level = level;
        Quality = quality;
        StatBonus = statBonus;
    }
}

[Serializable]
public class GameData
{
    public List<GameDataEntry> Entries;

    public GameData()
    {
        Entries = new List<GameDataEntry>();
    }
}
