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

    // Json���� ��ȯ�� ������
    public Dictionary<string, ItemClass> toJsonData = new Dictionary<string, ItemClass>();

    #region Inven ���� ������Ƽ
    // Json���� ��ȯ�� ������
    private Dictionary<string, ItemClass> toInvenItemDatas = new Dictionary<string, ItemClass>();
    // ItemListManager���� ������� Button GameObject ���� ����Ʈ
    public List<GameObject> InvenButtonList = new List<GameObject>();
    private string invenJsonFileName = "invendata.json";
    // �� �κ��� ���� �׸��带 ������ 2���� �迭 (InvenGridManager �����ͼ� �ʿ����)
    // public GameObject[,] slotGrid;
    // �� �κ��丮 �Ŵ��� ����.
    // List�� Inspectorâ�� ��Ÿ���� ������� ���.
    public List<InvenGridManager> list_IGMs = new List<InvenGridManager>();
    public Dictionary<string, InvenGridManager> IGMs = new Dictionary<string, InvenGridManager>();

    // *************** ������ ���� Json ���� ������Ƽ ***************
    private string invenPositionJsonFileName = "invenpositiondata.json";
    public List<string> invenPosData = new List<string>();
    // ��ư ������Ʈ�� ��ġ string�� ������ Dictionary.
    public Dictionary<string, (GameObject, string)> totalInvenData = new Dictionary<string, (GameObject, string)>();
    #endregion

    #region �ɸ��� ���� ���� Property
    
    private string CharSaveStatJsonFile = "CharStat.json";

    //Json���� �ҷ��� Data ������ ���� Dictionary Container

    // UserID�� Name ����
    public Dictionary<string, string> CharStrProp = new Dictionary<string, string>();

    // ������ ���� ����
    public Dictionary<string, int> CharIntProp = new Dictionary<string, int>();

    // ��� ���� �߰� ���� �����̳�
    public Dictionary<string, int>  EquipBonusStat = new Dictionary<string, int>
    {
        { "Str", 0 },
        { "Dex", 0 },
        { "Vital", 0 },
        { "Mana", 0 }
    };

    // ��� ������Ʈ�� StatPopupView�� �ѷ��� �̺�Ʈ.
    public event Action OnInventoryPositionUpdated;
    #endregion

    #region �ɸ��� ��ų ���� Property

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
        // JSON ������ Resources �������� �б�
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(jsonFileName));
        if (jsonTextAsset != null)
        {
            // JSON �����͸� GameData ��ü�� ��ȯ
            GameData loadedData = JsonUtility.FromJson<GameData>(jsonTextAsset.text);

            // �ҷ��� ������ ���
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
        //Start �� �ڷ��ġ������ Clear.
        //startItemList.Clear();

        // �׽�Ʈ�� ���� �޼��� ����.
        //StartCoroutine(LoadToInvenData());
    }

    #region List �߰� / ���� / ���� ���� ���
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
            //Json �߰� ������ ���� Dictionary �ڷ��� �߰�.
            toJsonData.Add(uniqueId.ToString(), item);
        }
    }

    public void DeleteAndModifyJsonData(string uniqueKey)
    {
        toJsonData.Remove(uniqueKey);

        // Dictionary �����͸� GameData �������� ��ȯ
        GameData newGameData = ConvertToGameData(toJsonData);

        // JSON ���� ��� ���� (Resources ���� ���� ��η� ����)
        jsonFilePath = Path.Combine(Application.dataPath, "Resources", jsonFileName);

        // JSON ���Ϸ� ����
        SaveToJson(newGameData, jsonFilePath);
    }

    public void AddItemJsonData(ItemClass item)
    {
        // Dictionary�� �׸� �߰�
        Guid uniqueId = Guid.NewGuid();
        toJsonData.Add(uniqueId.ToString(), item);

        // Dictionary �����͸� GameData �������� ��ȯ
        GameData newGameData = ConvertToGameData(toJsonData);

        // JSON ���� ��� ���� (Resources ���� ���� ��η� ����)
        jsonFilePath = Path.Combine(Application.dataPath, "Resources", jsonFileName);

        // JSON ���Ϸ� ����
        SaveToJson(newGameData, jsonFilePath);
    }

    private GameData ConvertToGameData(Dictionary<string, ItemClass> data)
    {
        GameData gameData = new GameData();
        GameDataEntry currentEntry = new GameDataEntry();

        foreach (var item in data.Values)
        {
            // ���� Entry�� 5�� �̻��� �׸��� �ִ� ��� ���ο� Entry�� ����
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

        // ������ Entry �߰�
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

    #region Inven �� EquipInven �߰� / ���� / ���� ���� ���
    // "invendata.json"���� ����� ������ ������ �ҷ�����.
    public void LoadToInvenData()
    {
        //yield return new WaitForSecondsRealtime(1.0f);

        // InvenGridManager���� slotGrid �����ͼ� Dictionary�� ����.
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
            // JSON �����͸� GameData ��ü�� ��ȯ
            GameData loadedData = JsonUtility.FromJson<GameData>(jsonTextAsset.text);

            // �ҷ��� �����Ͱ� null���� Ȯ���ϰ� ó��
            if (loadedData != null && loadedData.Entries != null)
            {
                // �ҷ��� ������ ���
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

    // �ҷ��� ������ toInvenItemDatas Dictionary�� ����
    public void CrateInvenItemDatas(GameDataEntry entry)
    {
        for (int i = 0; i < entry.GlobalIDs.Count; i++)
        {
            ItemClass item = new ItemClass();
            ItemClass.SetItemValues(item, entry.GlobalIDs[i], entry.Levels[i], entry.Qualities[i]);
            ItemClass.SetItemValues(item, entry.StatBonuses[i]);
            Guid uniqueId = Guid.NewGuid();
            item.UniqueKey = uniqueId.ToString();
            //Json �߰� ������ ���� Dictionary �ڷ��� �߰�.
            toInvenItemDatas.Add(uniqueId.ToString(), item);
        }
    }

    // �޾ƿ� �κ� ������ ��ư(Object) �����.
    public void MakeInvenObjects()
    {
        // ������ ������ ���ʿ� ����.
        if (toInvenItemDatas.Count <= 0)
            return;

        foreach (var itemData in toInvenItemDatas)
        {
            // InvenButtonList�� Btn��� �����ϱ�.
            listManager.ForInvenAddButton(itemData.Value);
        }

        // invenPosData����.
        InvenItemsPositionDataToJson();

        // InvenButtonList�� invenPosData�� ���ļ� �����ϴ�
        // UniqueKey�� Ű���ϴ� Dictionary (totalInvenData) �����.
        // GameObject�� string�� Ʃ�÷� ���� ����.
        for (int i = 0; i < InvenButtonList.Count; i++)
        {
            string key = InvenButtonList[i].transform.GetComponent<ItemButtonScript>().item.UniqueKey;
            totalInvenData.Add(key, (InvenButtonList[i], invenPosData[i]));
        }

        InvenAndEquipMakePosions();
    }

    // invenPositionJsonFileName���� ��ġ�� �������� �޼ҵ�
    public void InvenItemsPositionDataToJson()
    {
        // JSON ������ Resources �������� �б�
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(invenPositionJsonFileName));
        if (jsonTextAsset != null)
        {
            // JSON �����͸� GameInvenData ��ü�� ��ȯ
            GameInvenData loadedData = JsonUtility.FromJson<GameInvenData>(jsonTextAsset.text);

            // �ҷ��� ������ ���
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

    // totalInvenData �����̳� ��ȸ�ϸ鼭 �κ��� ��� ������ ��ġ��Ű�� �޼ҵ�
    public void InvenAndEquipMakePosions()
    {
        foreach (var data in totalInvenData)
        {
            GameObject invenButton = data.Value.Item1;
            string[] invenArr = data.Value.Item2.Split("/");

            // ������ Btn ��� �����ϱ�
            ItemButtonScript ibs = invenButton.transform.GetComponent<ItemButtonScript>();
            // Spawn�ؼ� ���� ���ٴϴ� ����
            ibs.SpawnStoredItem();

            IGMs[invenArr[0]].InvenDataPostioning(int.Parse(invenArr[1]), int.Parse(invenArr[2]));

            // ��� Ż���� ���ʽ� ���� ������ ���� ������ �����̳ʿ� ��Ƶ�.
            // �����Ҷ� ����.
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

    // totalInvenData ��ųʸ��� �����͸� ������� invendata.json ������ ������Ʈ�ϴ� �޼����Դϴ�.
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

    // totalInvenData ��ųʸ��� �����͸� ������� invenpositiondata.json ������ ������Ʈ�ϴ� �޼����Դϴ�.
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
        // ������ ������
        ItemClass itemClass = null;

        if (gameObject.TryGetComponent<ItemButtonScript>(out ItemButtonScript itemButtonScript))
            itemClass = gameObject.GetComponent<ItemButtonScript>().item;
        else
            itemClass = gameObject.transform.GetComponent<ItemScript>().item;

        // ���� �������� �κ��� �ƴϰ� �Ű��� �������� �κ��̸� �׸�ŭ ����.
        if (post_InvenPosition.Equals("Inven") == false && after_InvenPosition.Equals("Inven") == true)
        {
            EquipBonusStat["Str"] -= itemClass.Str;
            EquipBonusStat["Dex"] -= itemClass.Dex;
            EquipBonusStat["Vital"] -= itemClass.Vital;
            EquipBonusStat["Mana"] -= itemClass.Mana;
        }// ���� �������� �κ��̰� �Ű��� �������� �κ��� �ƴϸ� �׸�ŭ �߰�.
        else if(post_InvenPosition.Equals("Inven") == true && after_InvenPosition.Equals("Inven") == false)
        {
            EquipBonusStat["Str"] += itemClass.Str;
            EquipBonusStat["Dex"] += itemClass.Dex;
            EquipBonusStat["Vital"] += itemClass.Vital;
            EquipBonusStat["Mana"] += itemClass.Mana;
        }
    }

    // uniqueKey�� ����Ͽ� totalInvenData���� �ش� �׸��� �����ϰ�, ���ο� ��ġ ������ ������ ��, �ٽ� JSON ���Ͽ� ����
    public void UpdateInvenItemPositionJson(string uniqueKey, string pPositionData)
    {
        if (!totalInvenData.ContainsKey(uniqueKey))
        {
            Debug.LogWarning($"Item with UniqueKey {uniqueKey} not found in totalInvenData.");
            return;
        }

        // ���� �κ� ����.
        var tupleData = totalInvenData[uniqueKey];
        string post_InvenPosition = tupleData.Item2.Split('/')[0];
        string after_InvenPosition = pPositionData.Split('/')[0];

        // �ش� �׸��� totalInvenData���� ������ �� ����
        var (gameObject, _) = totalInvenData[uniqueKey];
        totalInvenData.Remove(uniqueKey);

        // ���ο� ��ġ ������ ������ �׸��� totalInvenData�� �߰�
        totalInvenData.Add(uniqueKey, (gameObject, pPositionData));

        ChangeEquipBonusStat(post_InvenPosition, after_InvenPosition, gameObject);

        // GameInvenData ��ü ���� �� ������Ʈ�� ������ �߰�
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

        // JSON ���� ��� ����
        string filePath = Path.Combine(Application.dataPath, "Resources", invenPositionJsonFileName);

        // JSON ���Ϸ� ����
        SaveToJson(newInvenData, filePath);

        // ��� ������Ʈ�� StatPopupView�� �ѷ��� �̺�Ʈ.
        // �̺�Ʈ Ʈ����
        OnInventoryPositionUpdated?.Invoke();
    }

    // uniqueKey�� ����Ͽ� totalInvenData���� �ش� ������ ������ ������ ��, �ٽ� JSON ���Ͽ� ����
    public void AddInvenItemPositionJson(string uniqueKey, string pPositionData, GameObject btnObj)
    {
        // ���ο� ��ġ ������ ������ �׸��� totalInvenData�� �߰�

        //ItemClass itc = btnObj.transform.GetComponent<ItemScript>().item;

        //btnObj = listManager.ForInvenDataBtnObjMaker(itc);

        totalInvenData.Add(uniqueKey, (btnObj, pPositionData));

        // Update the JSON files
        UpdateInvenDataJson();
        UpdateInvenPositionJson();

        // GameInvenData ��ü ���� �� ������Ʈ�� ������ �߰�
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
        //// JSON ���� ��� ����
        //string filePath = Path.Combine(Application.dataPath, "Resources", invenPositionJsonFileName);
        //
        //// JSON ���Ϸ� ����
        //SaveToJson(newInvenData, filePath);
    }

    // Ư�� �׸��� totalInvenData���� �����ϰ� JSON ������ ������Ʈ�ϴ� �޼����Դϴ�.
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

    #region �ɸ��� ���� �� ��ų �߰� / ���� ���� ���

    // Json File Read�ؿ���.
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
                    // Key�� ������ ���� ���������� ����.
                    current_UserID = character.UserId;

                    // CharacterClassString�� CharacterClass�� ��ȯ
                    character.ConvertStringToEnum();

                    // �⺻ ���� �ʱ�ȭ
                    InitializeStats(character);

                    // JSON���� �ε�� ������ ����
                    CharStrProp[character.UserId] = character.Name;
                    CharIntProp[$"{character.UserId}_Strength"] += character.Strength + EquipBonusStat["Str"];
                    CharIntProp[$"{character.UserId}_Dexterity"] += character.Dexterity + EquipBonusStat["Dex"];
                    CharIntProp[$"{character.UserId}_Vitality"] += character.Vitality + EquipBonusStat["Vital"];
                    CharIntProp[$"{character.UserId}_Energy"] += character.Energy + EquipBonusStat["Mana"];
                    CharIntProp[$"{character.UserId}_Level"] = character.Level;
                    CharIntProp[$"{character.UserId}_CurrentExp"] = character.CurrentExp;
                    CharIntProp[$"{character.UserId}_StatPoints"] = character.StatPoints;
                    CharIntProp[$"{character.UserId}_CharacterClass"] = (int)character.CharacterClass;

                    // �Ļ��Ǵ� ���� ���
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
        yield return null;  // �ڷ�ƾ�� ���������� ����
    }

    public void LoadCharacterStats(Action onComplete)
    {
        StartCoroutine(CharacterStatLoadCoroutine(onComplete));
    }

    // �ʱⰪ ����
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

    // �Ļ����� ��� ���
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

    // ����ġ ��� ���
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

    // ���� ���� ���
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

            // ������ �ε尡 �Ϸ�Ǿ��� �� ����� �ݹ�
            Action onComplete = () => { isLoaded = true; };

            // ĳ���� ���� �ε� ����
            JsonDataManager.Instance.LoadCharacterStats(onComplete);

            // ������ �ε尡 �Ϸ�� ������ ��ٸ�
            while (!isLoaded)
            {
                yield return null;
            }

            // ������ �ε尡 �Ϸ�� �� ����
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
