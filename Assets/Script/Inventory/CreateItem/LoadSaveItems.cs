using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSaveItems : MonoBehaviour
{

    public LoadItemDatabase itemDB;
    public ItemListManager listManager;

    public TextAsset startItemsFile;
    public TextAsset presetItemsFile;
    public TextAsset saveFile;

    private List<ItemClass> startItemList = new List<ItemClass>();

    private void Start()
    {
        startItemList = LoadItems(startItemsFile); //create and initialize startItemList on listManager;
        listManager.startItemList = this.startItemList;
    }

    public List<ItemClass> LoadItems(TextAsset itemFile)
    {
        string[][] grid = CsvReadWrite.LoadTextFile(itemFile);
        List<ItemClass> itemList = new List<ItemClass>();
        for (int i = 1; i < grid.Length; i++)
        {
            if (grid[i].Length == 0 || string.IsNullOrWhiteSpace(grid[i][0]))
            {
                // ºó ÁÙ ¶Ç´Â Ã¹ ¹øÂ° ¼¿ÀÌ ºó °æ¿ì ¹«½Ã
                continue;
            }

            ItemClass item = new ItemClass();
            ItemClass.SetItemValues(item, Int32.Parse(grid[i][0]), Int32.Parse(grid[i][1]), Int32.Parse(grid[i][2]));
            itemList.Add(item);
        }
        return itemList;
    }

    public void SaveItemsToFile(List<ItemClass> itemList, TextAsset file)
    {

    }

}
