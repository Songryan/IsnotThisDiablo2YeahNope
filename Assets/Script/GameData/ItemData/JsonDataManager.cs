using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class JsonDataManager : MonoBehaviour
{
    [SerializeField] private LoadItemDatabase itemDB;
    [SerializeField] private ItemListManager listManager;

    private List<ItemClass> startItemList = new List<ItemClass>();

    private string jsonFileName = "gamedata.json";
    private string jsonFilePath;

    // Json에서 변환된 데이터
    public Dictionary<string, ItemClass> toJsonData = new Dictionary<string, ItemClass>();

    #region Inven 관련 프로퍼티
    // Json에서 변환된 데이터
    private Dictionary<string, ItemClass> toInvenItemDatas = new Dictionary<string, ItemClass>();
    // ItemListManager에서 만들어진 Button GameObject 저장 리스트
    public List<GameObject> InvenButtonList = new List<GameObject>();
    private string invenJsonFileName = "invendata.json";
    // 각 인벤의 슬롯 그리드를 저장할 2차원 배열 (InvenGridManager 가져와서 필요없음)
    // public GameObject[,] slotGrid;
    // 각 인벤토리 매니저 저장.
    // List는 Inspector창에 나타나서 담기위해 사용.
    public List<InvenGridManager> list_IGMs = new List<InvenGridManager>();
    public Dictionary<string, InvenGridManager> IGMs = new Dictionary<string, InvenGridManager>();

    // *************** 포지션 저장 Json 관련 프로퍼티 ***************
    private string invenPositionJsonFileName = "invenpositiondata.json";
    public List<string> invenPosData = new List<string>();
    // 버튼 오브젝트와 위치 string을 가지는 Dictionary.
    public Dictionary<string, (GameObject, string)> totalInvenData = new Dictionary<string, (GameObject, string)>();
    #endregion

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

    public void LoadToGameData()
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
        //StartCoroutine(LoadToInvenData());
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

    private void SaveToJson<T>(T data, string filePath)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }
    #endregion

    #region Inven 및 EquipInven 추가 / 삭제 / 수정 관련 기능
    // "invendata.json"에서 저장된 아이템 데이터 불러오기.
    public void LoadToInvenData()
    {
        //yield return new WaitForSecondsRealtime(1.0f);

        // InvenGridManager별로 slotGrid 가져와서 Dictionary로 저장.
        if (IGMs.Count > 0)
            return;

        foreach (var IGM in list_IGMs)
        {
            string key = string.Empty;

            if (IGM.gameObject.name.Equals("item"))
            {
                key = IGM.transform.parent.name;
            }
            else
            {
                key = "Inven";
            }

            IGMs.Add(key, IGM);
        }

        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(invenJsonFileName));
        if (jsonTextAsset != null)
        {
            // JSON 데이터를 GameData 객체로 변환
            GameData loadedData = JsonUtility.FromJson<GameData>(jsonTextAsset.text);

            // 불러온 데이터가 null인지 확인하고 처리
            if (loadedData != null && loadedData.Entries != null)
            {
                // 불러온 데이터 출력
                foreach (var entry in loadedData.Entries)
                {
                    CrateInvenItemDatas(entry);
                }
            }
            else
            {
                Debug.LogWarning("Loaded data or Entries is null");
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

    // 받아온 인벤 정보로 버튼(Object) 만들고.
    public void MakeInvenObjects()
    {
        // 데이터 없으면 돌필요 없음.
        if (toInvenItemDatas.Count <= 0)
            return;

        foreach (var itemData in toInvenItemDatas)
        {
            // InvenButtonList에 Btn목록 저장하기.
            listManager.ForInvenAddButton(itemData.Value);
        }

        // invenPosData생성.
        InvenItemsPositionDataToJson();

        // InvenButtonList랑 invenPosData랑 합쳐서 관리하는
        // UniqueKey를 키로하는 Dictionary (totalInvenData) 만들기.
        // GameObject랑 string을 튜플로 만들어서 관리.
        for (int i = 0; i < InvenButtonList.Count; i++)
        {
            string key = InvenButtonList[i].transform.GetComponent<ItemButtonScript>().item.UniqueKey;
            totalInvenData.Add(key, (InvenButtonList[i], invenPosData[i]));
        }

        InvenAndEquipMakePosions();
    }

    // invenPositionJsonFileName에서 위치값 가져오는 메소드
    public void InvenItemsPositionDataToJson()
    {
        // JSON 파일을 Resources 폴더에서 읽기
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(invenPositionJsonFileName));
        if (jsonTextAsset != null)
        {
            // JSON 데이터를 GameInvenData 객체로 변환
            GameInvenData loadedData = JsonUtility.FromJson<GameInvenData>(jsonTextAsset.text);

            // 불러온 데이터 출력
            foreach (var entry in loadedData.Entries)
            {
                for (int i = 0; i < entry.GridTypeKeys.Count; i++)
                {
                    invenPosData.Add($"{entry.GridTypeKeys[i]}/{entry.GridXs[i]}/{entry.GridYs[i]}");
                }
            }
        }
        else
        {
            Debug.LogWarning("JSON file not found in Resources");
        }
    }

    // totalInvenData 컨테이너 순회하면서 인벤과 장비에 아이템 위치시키는 메소드
    public void InvenAndEquipMakePosions()
    {
        foreach (var data in totalInvenData)
        {
            GameObject invenButton = data.Value.Item1;
            string[] invenArr = data.Value.Item2.Split("/");

            // 저장한 Btn 목록 생성하기
            ItemButtonScript ibs = invenButton.transform.GetComponent<ItemButtonScript>();
            // Spawn해서 동동 떠다니는 상태
            ibs.SpawnStoredItem();

            IGMs[invenArr[0]].InvenDataPostioning(int.Parse(invenArr[1]), int.Parse(invenArr[2]));
        }
    }

    // totalInvenData 딕셔너리의 데이터를 기반으로 invendata.json 파일을 업데이트하는 메서드입니다.
    private void UpdateInvenDataJson()
    {
        GameData newGameData = new GameData();
        GameDataEntry currentEntry = new GameDataEntry();

        foreach (var item in totalInvenData.Values)
        {
            ItemClass itemClass = null;

            if (item.Item1.TryGetComponent<ItemButtonScript>(out ItemButtonScript itemButtonScript))
                itemClass = item.Item1.GetComponent<ItemButtonScript>().item;
            else
                itemClass = item.Item1.transform.GetComponent<ItemScript>().item;

            if (currentEntry.GlobalIDs.Count >= 5)
            {
                newGameData.Entries.Add(currentEntry);
                currentEntry = new GameDataEntry();
            }

            currentEntry.GlobalIDs.Add(itemClass.GlobalID);
            currentEntry.Levels.Add(itemClass.Level);
            currentEntry.Qualities.Add(itemClass.qualityInt);
            currentEntry.StatBonuses.Add($"{itemClass.Str}/{itemClass.Dex}/{itemClass.Vital}/{itemClass.Mana}");
        }

        if (currentEntry.GlobalIDs.Count > 0)
        {
            newGameData.Entries.Add(currentEntry);
        }

        string filePath = Path.Combine(Application.dataPath, "Resources", invenJsonFileName);
        SaveToJson(newGameData, filePath);
    }

    // totalInvenData 딕셔너리의 데이터를 기반으로 invenpositiondata.json 파일을 업데이트하는 메서드입니다.
    private void UpdateInvenPositionJson()
    {
        GameInvenData newInvenData = new GameInvenData();
        GameInvenDataEntry currentEntry = new GameInvenDataEntry();

        foreach (var item in totalInvenData.Values)
        {
            string[] positionData = item.Item2.Split('/');
            string gridTypeKey = positionData[0];
            int gridX = int.Parse(positionData[1]);
            int gridY = int.Parse(positionData[2]);

            if (currentEntry.GridTypeKeys.Count >= 5)
            {
                newInvenData.Entries.Add(currentEntry);
                currentEntry = new GameInvenDataEntry();
            }

            currentEntry.GridTypeKeys.Add(gridTypeKey);
            currentEntry.GridXs.Add(gridX);
            currentEntry.GridYs.Add(gridY);
        }

        if (currentEntry.GridTypeKeys.Count > 0)
        {
            newInvenData.Entries.Add(currentEntry);
        }

        string filePath = Path.Combine(Application.dataPath, "Resources", invenPositionJsonFileName);
        SaveToJson(newInvenData, filePath);
    }

    // uniqueKey를 사용하여 totalInvenData에서 해당 항목을 삭제하고, 새로운 위치 정보를 적용한 후, 다시 JSON 파일에 저장
    public void UpdateInvenItemPositionJson(string uniqueKey, string pPositionData)
    {
        if (!totalInvenData.ContainsKey(uniqueKey))
        {
            Debug.LogWarning($"Item with UniqueKey {uniqueKey} not found in totalInvenData.");
            return;
        }

        // 해당 항목을 totalInvenData에서 가져온 후 삭제
        var (gameObject, _) = totalInvenData[uniqueKey];
        totalInvenData.Remove(uniqueKey);

        // 새로운 위치 정보를 적용한 항목을 totalInvenData에 추가
        totalInvenData.Add(uniqueKey, (gameObject, pPositionData));

        // GameInvenData 객체 생성 및 업데이트된 데이터 추가
        GameInvenData newInvenData = new GameInvenData();
        GameInvenDataEntry currentEntry = new GameInvenDataEntry();

        foreach (var item in totalInvenData.Values)
        {
            string[] posDataParts = item.Item2.Split('/');
            string gridTypeKey = posDataParts[0];
            int gridX = int.Parse(posDataParts[1]);
            int gridY = int.Parse(posDataParts[2]);

            if (currentEntry.GridTypeKeys.Count >= 5)
            {
                newInvenData.Entries.Add(currentEntry);
                currentEntry = new GameInvenDataEntry();
            }

            currentEntry.GridTypeKeys.Add(gridTypeKey);
            currentEntry.GridXs.Add(gridX);
            currentEntry.GridYs.Add(gridY);
        }

        if (currentEntry.GridTypeKeys.Count > 0)
        {
            newInvenData.Entries.Add(currentEntry);
        }

        // JSON 파일 경로 설정
        string filePath = Path.Combine(Application.dataPath, "Resources", invenPositionJsonFileName);

        // JSON 파일로 저장
        SaveToJson(newInvenData, filePath);
    }

    // uniqueKey를 사용하여 totalInvenData에서 해당 아이템 정보를 저장한 후, 다시 JSON 파일에 저장
    public void AddInvenItemPositionJson(string uniqueKey, string pPositionData, GameObject btnObj)
    {
        // 새로운 위치 정보를 적용한 항목을 totalInvenData에 추가

        //ItemClass itc = btnObj.transform.GetComponent<ItemScript>().item;

        //btnObj = listManager.ForInvenDataBtnObjMaker(itc);

        totalInvenData.Add(uniqueKey, (btnObj, pPositionData));

        // Update the JSON files
        UpdateInvenDataJson();
        UpdateInvenPositionJson();

        // GameInvenData 객체 생성 및 업데이트된 데이터 추가
        //GameInvenData newInvenData = new GameInvenData();
        //GameInvenDataEntry currentEntry = new GameInvenDataEntry();
        //
        //foreach (var item in totalInvenData.Values)
        //{
        //    string[] posDataParts = item.Item2.Split('/');
        //    string gridTypeKey = posDataParts[0];
        //    int gridX = int.Parse(posDataParts[1]);
        //    int gridY = int.Parse(posDataParts[2]);
        //
        //    if (currentEntry.GridTypeKeys.Count >= 5)
        //    {
        //        newInvenData.Entries.Add(currentEntry);
        //        currentEntry = new GameInvenDataEntry();
        //    }
        //
        //    currentEntry.GridTypeKeys.Add(gridTypeKey);
        //    currentEntry.GridXs.Add(gridX);
        //    currentEntry.GridYs.Add(gridY);
        //}
        //
        //if (currentEntry.GridTypeKeys.Count > 0)
        //{
        //    newInvenData.Entries.Add(currentEntry);
        //}
        //
        //// JSON 파일 경로 설정
        //string filePath = Path.Combine(Application.dataPath, "Resources", invenPositionJsonFileName);
        //
        //// JSON 파일로 저장
        //SaveToJson(newInvenData, filePath);
    }

    // 특정 항목을 totalInvenData에서 제거하고 JSON 파일을 업데이트하는 메서드입니다.
    public void DeleteItemFromJson(string uniqueKey)
    {
        if (totalInvenData.ContainsKey(uniqueKey))
        {
            totalInvenData.Remove(uniqueKey);

            // Update the JSON files
            UpdateInvenDataJson();
            UpdateInvenPositionJson();
        }
        else
        {
            Debug.LogWarning($"Item with UniqueKey {uniqueKey} not found in totalInvenData.");
        }
    }
    #endregion
}
