using System;
using UnityEngine;

[System.Serializable]
public class ItemClass
{
    public int GlobalID;
    [HideInInspector] public int CategoryID;
    [HideInInspector] public string CategoryName;
    [HideInInspector] public int TypeID;
    public string TypeName;
    [Range(1, 100)] public int Level;
    [Range(0, 3)] public int qualityInt;
    [HideInInspector] public IntVector2 Size;
    [HideInInspector] public Sprite Icon;
    [HideInInspector] public string SerialID;

    // 아이템 스텟보너스 추가
    public int Str;
    public int Dex;
    public int Vital;
    public int Mana;

    private enum QualityEnum { Broken, Normal, Magic, Rare }
    public string GetQualityStr()
    {
        return Enum.GetName(typeof(QualityEnum), qualityInt);
    }

    public static void SetItemValues(ItemClass item, int ID, int lvl, int quality)
    {
        item.GlobalID = ID;
        item.Level = lvl;
        item.qualityInt = quality;
        GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<LoadItemDatabase>().PassItemData(ref item);
    }

    public static void SetItemValues(ItemClass item)
    {
        GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<LoadItemDatabase>().PassItemData(ref item);
    }

    public static void SetItemValues(ItemClass item, string statBouns)
    {
        string[] statsArr = statBouns.Split('/');
        item.Str = Int32.Parse(statsArr[0]);
        item.Dex = Int32.Parse(statsArr[1]);
        item.Vital = Int32.Parse(statsArr[2]);
        item.Mana = Int32.Parse(statsArr[3]);
    }

    public ItemClass(ItemClass passedItem)//create new item by copying passedITem properties
    {
        GlobalID = passedItem.GlobalID;
        Level = passedItem.Level;
        qualityInt = passedItem.qualityInt;
    }
    public ItemClass() { }//creates error if this is not put. dont know why

}
