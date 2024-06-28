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
    private Dictionary<string, ItemClass> toJsonData = new Dictionary<string, ItemClass>();

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
    public Dictionary<string,InvenGridManager> IGMs = new Dictionary<string,InvenGridManager>();

    // *************** 포지션 저장 Json 관련 프로퍼티 ***************
    private string invenPositionJsonFileName = "invenpositiondata.json";
    public List<string> invenPosData = new List<string>();
    // 버튼 오브젝트와 위치 string을 가지는 Dictionary.
    public Dictionary<string, (GameObject, string)> totalInvenData;
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

        // InvenGridManager별로 slotGrid 가져와서 Dictionary로 저장.
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

    // 받아온 인벤 정보로 버튼(Object) 만들고.
    public void MakeInvenObjects()
    {
        // 데이터 없으면 돌필요 없음.
        if(toInvenItemDatas.Count <= 0)
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


        //for(int i = 0; i < InvenButtonList.Count; i++)
        //{
        //    // 저장한 Btn 목록 생성하기
        //    ItemButtonScript ibs = InvenButtonList[i].transform.GetComponent<ItemButtonScript>();
        //    // Spawn해서 동동 떠다니는 상태
        //    ibs.SpawnStoredItem();
        //
        //    //받아온 Grid 인벤 string 정보
        //    // 예 : 인벤
        //    //string gridTypeKey = "Inven";
        //    // 저장된 Start Grid Position 정보.
        //    //int gridX = 0;
        //    //int gridY = 0;
        //
        //    // 포지션 정보 가져오기.
        //    // public List<string> invenPosData에 저장.
        //    InvenItemsPositionDataToJson();
        //
        //    string[] arr = invenPosData[i].Split("/");
        //
        //    // InvenDataPostioning(int x, int y) 실행시켜서 배치시키기.
        //    IGMs[arr[0]].InvenDataPostioning(int.Parse(arr[1]), int.Parse(arr[2]));
        //}
    }

    // invenPositionJsonFileName에서 위치값 가져오는 메소드
    public void InvenItemsPositionDataToJson()
    {
        // JSON 파일을 Resources 폴더에서 읽기
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(invenPositionJsonFileName));
        if (jsonTextAsset != null)
        {
            // JSON 데이터를 GameData 객체로 변환
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
        foreach(var data in totalInvenData)
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
    #endregion
}
