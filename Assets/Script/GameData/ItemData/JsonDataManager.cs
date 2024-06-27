using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonDataManager : MonoBehaviour
{
    [SerializeField] private LoadItemDatabase itemDB;
    [SerializeField] private ItemListManager listManager;

    private List<ItemClass> startItemList = new List<ItemClass>();

    private string jsonFileName = "gamedata.json";
    private string invenJsonFileName = "invendata.json";
    private string jsonFilePath;

    // Json에서 변환된 데이터
    private Dictionary<string, ItemClass> toJsonData = new Dictionary<string, ItemClass>();
    // Json에서 변환된 데이터
    private Dictionary<string, ItemClass> toInvenItemDatas = new Dictionary<string, ItemClass>();
    // ItemListManager에서 만들어진 Button GameObject 저장 리스트
    public List<GameObject> InvenButtonList;

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

            // 불러온 데이터 출력
            foreach (var entry in loadedData.Entries)
            {
                LoadItems(entry);
            }
        }
        else
        {
            Debug.LogWarning("JSON file not found in Resources");
        }

        listManager.startItemList = this.startItemList;
        //Start 후 자료배치했으면 Clear.
        //startItemList.Clear();

        // 테스트를 위한 메서드 실행.
        StartCoroutine(LoadToInvenData());
    }

    #region List 추가 / 삭제 / 수정 관련 기능
    public void LoadItems(GameDataEntry entry)
    {
        for (int i = 0; i < entry.GlobalIDs.Count; i++)
        {
            ItemClass item = new ItemClass();
            ItemClass.SetItemValues(item, entry.GlobalIDs[i], entry.Levels[i], entry.Qualities[i]);
            ItemClass.SetItemValues(item, entry.StatBonuses[i]);
            Guid uniqueId = Guid.NewGuid();
            item.UniqueKey = uniqueId.ToString();
            startItemList.Add(item);
            //Json 추가 삭제를 위한 Dictionary 자료형 추가.
            toJsonData.Add(uniqueId.ToString(), item);
        }
    }

    public void DeleteAndModifyJsonData(string uniqueKey)
    {
        // Dictionary에서 항목 제거
        toJsonData.Remove(uniqueKey);

        // Dictionary 데이터를 GameData 형식으로 변환
        GameData newGameData = ConvertToGameData(toJsonData);

        // JSON 파일 경로 설정 (Resources 폴더 내의 경로로 설정)
        jsonFilePath = Path.Combine(Application.dataPath, "Resources", jsonFileName);

        // JSON 파일로 저장
        SaveToJson(newGameData, jsonFilePath);
    }

    public void AddItemJsonData(ItemClass item)
    {
        // Dictionary에 항목 추가
        Guid uniqueId = Guid.NewGuid();
        toJsonData.Add(uniqueId.ToString(), item);

        // Dictionary 데이터를 GameData 형식으로 변환
        GameData newGameData = ConvertToGameData(toJsonData);

        // JSON 파일 경로 설정 (Resources 폴더 내의 경로로 설정)
        jsonFilePath = Path.Combine(Application.dataPath, "Resources", jsonFileName);

        // JSON 파일로 저장
        SaveToJson(newGameData, jsonFilePath);
    }

    private GameData ConvertToGameData(Dictionary<string, ItemClass> data)
    {
        GameData gameData = new GameData();
        GameDataEntry currentEntry = new GameDataEntry();

        foreach (var item in data.Values)
        {
            // 현재 Entry에 5개 이상의 항목이 있는 경우 새로운 Entry를 생성
            if (currentEntry.GlobalIDs.Count >= 5)
            {
                gameData.Entries.Add(currentEntry);
                currentEntry = new GameDataEntry();
            }

            currentEntry.GlobalIDs.Add(item.GlobalID);
            currentEntry.Levels.Add(item.Level);
            currentEntry.Qualities.Add(item.qualityInt);
            currentEntry.StatBonuses.Add($"{item.Str}/{item.Dex}/{item.Vital}/{item.Mana}");
        }

        // 마지막 Entry 추가
        if (currentEntry.GlobalIDs.Count > 0)
        {
            gameData.Entries.Add(currentEntry);
        }

        return gameData;
    }

    private void SaveToJson(GameData gameData, string filePath)
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(filePath, json);
    }
    #endregion

    #region Inven 및 EquipInven 추가 / 삭제 / 수정 관련 기능

    // "invendata.json"에서 저장된 아이템 데이터 불러오기.
    public IEnumerator LoadToInvenData()
    {
        yield return new WaitForSecondsRealtime(1.0f);

        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(invenJsonFileName));
        if (jsonTextAsset != null)
        {
            // JSON 데이터를 GameData 객체로 변환
            GameData loadedData = JsonUtility.FromJson<GameData>(jsonTextAsset.text);

            // 불러온 데이터 출력
            foreach (var entry in loadedData.Entries)
            {
                CrateInvenItemDatas(entry);
            }
        }
        else
        {
            Debug.LogWarning("JSON file not found in Resources");
        }

        MakeInvenObjects();
    }

    // 불러온 데이터 toInvenItemDatas Dictionary에 저장
    public void CrateInvenItemDatas(GameDataEntry entry)
    {
        for (int i = 0; i < entry.GlobalIDs.Count; i++)
        {
            ItemClass item = new ItemClass();
            ItemClass.SetItemValues(item, entry.GlobalIDs[i], entry.Levels[i], entry.Qualities[i]);
            ItemClass.SetItemValues(item, entry.StatBonuses[i]);
            Guid uniqueId = Guid.NewGuid();
            item.UniqueKey = uniqueId.ToString();
            //Json 추가 삭제를 위한 Dictionary 자료형 추가.
            toInvenItemDatas.Add(uniqueId.ToString(), item);
        }
    }

    // 받아온 인벤 정보로 버튼(Object) 만들기
    public void MakeInvenObjects()
    {
        foreach (var itemData in toInvenItemDatas)
        {
            // ItemListManager에서 public void AddButton(ItemClass addItem) 가져와서 쓰기.
            // AddButton(itemData); 로 일단 버튼 만들기.
            listManager.ForInvenAddButton(itemData.Value);
        }
    }




    #endregion
}
