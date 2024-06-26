using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonDataManager : MonoBehaviour
{
    [SerializeField] private LoadItemDatabase itemDB;
    [SerializeField] private ItemListManager listManager;

    private List<ItemClass> startItemList = new List<ItemClass>();

    private string jsonFileName = "gamedata.json";
    private string jsonFilePath;

    private static JsonDataManager _instance;
    public static JsonDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<JsonDataManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(JsonDataManager).ToString());
                    _instance = singletonObject.AddComponent<JsonDataManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // JSON 파일을 Resources 폴더에서 읽기
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(jsonFileName));
        if (jsonTextAsset != null)
        {
            // JSON 데이터를 GameData 객체로 변환
            GameData loadedData = JsonUtility.FromJson<GameData>(jsonTextAsset.text);

            // 불러온 데이터 출력 (확인용)
            foreach (var entry in loadedData.Entries)
            {
                LoadItems(entry);
                //for (int i = 0; i < entry.GlobalIDs.Count; i++)
                //{
                //    Debug.Log($"GlobalID: {entry.GlobalIDs[i]}, Level: {entry.Levels[i]}, Quality: {entry.Qualities[i]}, StatBonus: {entry.StatBonuses[i]}");
                //}
            }
        }
        else
        {
            Debug.LogWarning("JSON file not found in Resources");
        }

        listManager.startItemList = this.startItemList;
        //Start 후 자료배치했으면 Clear.
        //startItemList.Clear();
    }

    public void LoadItems(GameDataEntry entry)
    {
        for (int i = 1; i < entry.GlobalIDs.Count; i++)
        {
            ItemClass item = new ItemClass();
            ItemClass.SetItemValues(item, entry.GlobalIDs[i], entry.Levels[i], entry.Qualities[i]);
            ItemClass.SetItemValues(item, entry.StatBonuses[i]);
            startItemList.Add(item);
        }
    }
}
