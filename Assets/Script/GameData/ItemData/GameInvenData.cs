using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameInvenDataEntry
{
    public List<string> GridTypeKeys;
    public List<int> GridXs;
    public List<int> GridYs;

    public GameInvenDataEntry()
    {
        GridTypeKeys = new List<string>();
        GridXs = new List<int>();
        GridYs = new List<int>();
    }
}

[Serializable]
public class GameInvenData
{
    public List<GameInvenDataEntry> Entries;

    public GameInvenData()
    {
        Entries = new List<GameInvenDataEntry>();
    }
}
