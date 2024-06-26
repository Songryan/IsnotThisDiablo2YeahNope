using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameDataEntry
{
    public List<int> GlobalIDs;
    public List<int> Levels;
    public List<int> Qualities;
    public List<string> StatBonuses;

    public GameDataEntry()
    {
        GlobalIDs = new List<int>();
        Levels = new List<int>();
        Qualities = new List<int>();
        StatBonuses = new List<string>();
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
