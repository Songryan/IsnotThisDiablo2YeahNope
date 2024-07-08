using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.Progress;
using static UnityEngine.EventSystems.EventTrigger;
using TextAsset = UnityEngine.TextAsset;

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

    #region 케릭터 스텟 관련 Property
    
    private string CharSaveStatJsonFile = "CharStat.json";

    //Json에서 불러온 Data 저장을 위한 Dictionary Container

    // UserID와 Name 저장
    public Dictionary<string, string> CharStrProp = new Dictionary<string, string>();

    // 나머지 스탯 저장
    public Dictionary<string, int> CharIntProp = new Dictionary<string, int>();

    // 장비 착용 추가 스텟 컨테이너
    public Dictionary<string, int>  EquipBonusStat = new Dictionary<string, int>
    {
        { "Str", 0 },
        { "Dex", 0 },
        { "Vital", 0 },
        { "Mana", 0 }
    };

    // 장비 업데이트시 StatPopupView에 뿌려줄 이벤트.
    public event Action OnInventoryPositionUpdated;
    #endregion

    #region 케릭터 스킬 관련 Property

    private string CharSaveSkillJsonFile = "Skill.json";

    public string current_UserID = string.Empty;

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
            UIManager.Instance.EnableRootUI();
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

            // 장비 탈착시 보너스 스탯 적용을 위한 스텟을 컨테이너에 담아둠.
            // 시작할때 세팅.
            if (invenArr[0].Equals("Inven") == false)
            {
                ItemClass itemClass = null;

                if (data.Value.Item1.TryGetComponent<ItemButtonScript>(out ItemButtonScript itemButtonScript))
                    itemClass = data.Value.Item1.GetComponent<ItemButtonScript>().item;
                else
                    itemClass = data.Value.Item1.transform.GetComponent<ItemScript>().item;

                EquipBonusStat["Str"] += itemClass.Str;
                EquipBonusStat["Dex"] += itemClass.Dex;
                EquipBonusStat["Vital"] += itemClass.Vital;
                EquipBonusStat["Mana"] += itemClass.Mana;
            }
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

    public void ChangeEquipBonusStat(string post_InvenPosition, string after_InvenPosition, GameObject gameObject)
    {
        // 아이템 데이터
        ItemClass itemClass = null;

        if (gameObject.TryGetComponent<ItemButtonScript>(out ItemButtonScript itemButtonScript))
            itemClass = gameObject.GetComponent<ItemButtonScript>().item;
        else
            itemClass = gameObject.transform.GetComponent<ItemScript>().item;

        // 이전 포지션이 인벤이 아니고 옮겨진 포지션이 인벤이면 그만큼 차감.
        if (post_InvenPosition.Equals("Inven") == false && after_InvenPosition.Equals("Inven") == true)
        {
            EquipBonusStat["Str"] -= itemClass.Str;
            EquipBonusStat["Dex"] -= itemClass.Dex;
            EquipBonusStat["Vital"] -= itemClass.Vital;
            EquipBonusStat["Mana"] -= itemClass.Mana;
        }// 이전 포지션이 인벤이고 옮겨진 포지션이 인벤이 아니면 그만큼 추가.
        else if(post_InvenPosition.Equals("Inven") == true && after_InvenPosition.Equals("Inven") == false)
        {
            EquipBonusStat["Str"] += itemClass.Str;
            EquipBonusStat["Dex"] += itemClass.Dex;
            EquipBonusStat["Vital"] += itemClass.Vital;
            EquipBonusStat["Mana"] += itemClass.Mana;
        }
    }

    // uniqueKey를 사용하여 totalInvenData에서 해당 항목을 삭제하고, 새로운 위치 정보를 적용한 후, 다시 JSON 파일에 저장
    public void UpdateInvenItemPositionJson(string uniqueKey, string pPositionData)
    {
        if (!totalInvenData.ContainsKey(uniqueKey))
        {
            Debug.LogWarning($"Item with UniqueKey {uniqueKey} not found in totalInvenData.");
            return;
        }

        // 이전 인벤 정보.
        var tupleData = totalInvenData[uniqueKey];
        string post_InvenPosition = tupleData.Item2.Split('/')[0];
        string after_InvenPosition = pPositionData.Split('/')[0];

        // 해당 항목을 totalInvenData에서 가져온 후 삭제
        var (gameObject, _) = totalInvenData[uniqueKey];
        totalInvenData.Remove(uniqueKey);

        // 새로운 위치 정보를 적용한 항목을 totalInvenData에 추가
        totalInvenData.Add(uniqueKey, (gameObject, pPositionData));

        ChangeEquipBonusStat(post_InvenPosition, after_InvenPosition, gameObject);

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

        // 장비 업데이트시 StatPopupView에 뿌려줄 이벤트.
        // 이벤트 트리거
        OnInventoryPositionUpdated?.Invoke();
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

    #region 케릭터 스텟 및 스킬 추가 / 수정 관련 기능

    // Json File Read해오기.
    public IEnumerator CharacterStatLoadCoroutine(Action onComplete)
    {
        string filePath = Path.Combine(Application.dataPath, "Resources", CharSaveStatJsonFile);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            CharacterData characterData = JsonUtility.FromJson<CharacterData>(json);

            if (characterData != null && characterData.Characters != null)
            {
                foreach (var character in characterData.Characters)
                {
                    // Key를 꺼내기 위해 전역변수로 저장.
                    current_UserID = character.UserId;

                    // CharacterClassString을 CharacterClass로 변환
                    character.ConvertStringToEnum();

                    // 기본 스탯 초기화
                    InitializeStats(character);

                    // JSON에서 로드된 데이터 적용
                    CharStrProp[character.UserId] = character.Name;
                    CharIntProp[$"{character.UserId}_Strength"] += character.Strength + EquipBonusStat["Str"];
                    CharIntProp[$"{character.UserId}_Dexterity"] += character.Dexterity + EquipBonusStat["Dex"];
                    CharIntProp[$"{character.UserId}_Vitality"] += character.Vitality + EquipBonusStat["Vital"];
                    CharIntProp[$"{character.UserId}_Energy"] += character.Energy + EquipBonusStat["Mana"];
                    CharIntProp[$"{character.UserId}_Level"] = character.Level;
                    CharIntProp[$"{character.UserId}_CurrentExp"] = character.CurrentExp;
                    CharIntProp[$"{character.UserId}_StatPoints"] = character.StatPoints;
                    CharIntProp[$"{character.UserId}_CharacterClass"] = (int)character.CharacterClass;

                    // 파생되는 스탯 계산
                    CalculateAndStoreDerivedStats(character);
                }
            }
            else
            {
                Debug.LogWarning("Character data or Characters list is null");
            }
        }
        else
        {
            Debug.LogWarning("Character stats JSON file not found");
        }

        onComplete?.Invoke();
        yield return null;  // 코루틴이 정상적으로 종료
    }

    public void LoadCharacterStats(Action onComplete)
    {
        StartCoroutine(CharacterStatLoadCoroutine(onComplete));
    }

    // 초기값 세팅
    private void InitializeStats(CharacterStats character)
    {
        switch (character.CharacterClass)
        {
            case CharacterClass.Barbarian:
                CharIntProp[$"{character.UserId}_Strength"] = 30;
                CharIntProp[$"{character.UserId}_Dexterity"] = 20;
                CharIntProp[$"{character.UserId}_Vitality"] = 25;
                CharIntProp[$"{character.UserId}_Energy"] = 10;
                break;
            case CharacterClass.Amazon:
                CharIntProp[$"{character.UserId}_Strength"] = 20;
                CharIntProp[$"{character.UserId}_Dexterity"] = 25;
                CharIntProp[$"{character.UserId}_Vitality"] = 20;
                CharIntProp[$"{character.UserId}_Energy"] = 15;
                break;
            case CharacterClass.Paladin:
                CharIntProp[$"{character.UserId}_Strength"] = 25;
                CharIntProp[$"{character.UserId}_Dexterity"] = 20;
                CharIntProp[$"{character.UserId}_Vitality"] = 25;
                CharIntProp[$"{character.UserId}_Energy"] = 10;
                break;
        }
    }

    // 파생스텟 계산 기능
    private void CalculateAndStoreDerivedStats(CharacterStats character)
    {
        int life = CharIntProp[$"{character.UserId}_Vitality"] * 3;
        int stamina = CharIntProp[$"{character.UserId}_Vitality"] * 2;
        int mana = CharIntProp[$"{character.UserId}_Energy"] * 2;
        int damage = CharIntProp[$"{character.UserId}_Strength"];
        int attackRating = CharIntProp[$"{character.UserId}_Dexterity"] * 5;
        int defense = (int)(CharIntProp[$"{character.UserId}_Dexterity"] / 4);
        int chanceToBlock = CharIntProp[$"{character.UserId}_Dexterity"];
        int characterExp = CalculateLevelUpExp(character.Level);

        CharIntProp[$"{character.UserId}_Life"] = life;
        CharIntProp[$"{character.UserId}_Stamina"] = stamina;
        CharIntProp[$"{character.UserId}_Mana"] = mana;
        CharIntProp[$"{character.UserId}_Damage"] = damage;
        CharIntProp[$"{character.UserId}_AttackRating"] = attackRating;
        CharIntProp[$"{character.UserId}_Defense"] = defense;
        CharIntProp[$"{character.UserId}_ChanceToBlock"] = chanceToBlock;
        CharIntProp[$"{character.UserId}_CharacterExp"] = characterExp;
    }

    // 경험치 계산 기능
    private int CalculateLevelUpExp(int Level)
    {
        if (Level == 1)
        {
            return 100;
        }
        else
        {
            return (int)(100 * Math.Pow(1.5, Level - 1));
        }
    }

    // 스텟 수정 기능
    public void ModifyStatJsonDataSave(string stat)
    {
        string filePath = Path.Combine(Application.dataPath, "Resources", CharSaveStatJsonFile);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            
            CharacterData characterData = JsonUtility.FromJson<CharacterData>(json);

            if (characterData != null && characterData.Characters != null)
            {
                foreach (var character in characterData.Characters)
                {
                    if (character.StatPoints > 0)
                    {
                        switch (stat.ToLower())
                        {
                            case "strength":
                                character.Strength++;
                                break;
                            case "dexterity":
                                character.Dexterity++;
                                break;
                            case "vitality":
                                character.Vitality++;
                                break;
                            case "energy":
                                character.Energy++;
                                break;
                        }
                        character.StatPoints--;
                    }
                }
            }
            SaveToJson(characterData, filePath);
        }
    }
    #endregion
}


namespace UIData
{
    public static class UIDataExtension
    {
        public static void RefreshCharacterInfo(this JsonDataManager manager, Action<string, string, int> callback)
        {
            manager.StartCoroutine(manager.WaitForCharacterStats(callback));
        }

        public static IEnumerator WaitForCharacterStats(this JsonDataManager manager, Action<string, string, int> callback)
        {
            bool isLoaded = false;

            // 데이터 로드가 완료되었을 때 실행될 콜백
            Action onComplete = () => { isLoaded = true; };

            // 캐릭터 스탯 로드 시작
            JsonDataManager.Instance.LoadCharacterStats(onComplete);

            // 데이터 로드가 완료될 때까지 기다림
            while (!isLoaded)
            {
                yield return null;
            }

            // 데이터 로드가 완료된 후 실행
            string userID = string.Empty;
            string userName = string.Empty;
            foreach (var value in JsonDataManager.Instance.CharStrProp)
            {
                userID = value.Key;
                userName = value.Value;
            }

            callback.Invoke(userID, userName, JsonDataManager.Instance.CharIntProp[$"{userID}_Level"]);
        }
    }
}
