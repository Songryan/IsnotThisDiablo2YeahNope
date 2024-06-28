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

    // Json���� ��ȯ�� ������
    private Dictionary<string, ItemClass> toJsonData = new Dictionary<string, ItemClass>();

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
    public Dictionary<string,InvenGridManager> IGMs = new Dictionary<string,InvenGridManager>();

    // *************** ������ ���� Json ���� ������Ƽ ***************
    private string invenPositionJsonFileName = "invenpositiondata.json";
    public List<string> invenPosData = new List<string>();
    // ��ư ������Ʈ�� ��ġ string�� ������ Dictionary.
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
        StartCoroutine(LoadToInvenData());
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
        // Dictionary���� �׸� ����
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

    private void SaveToJson(GameData gameData, string filePath)
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(filePath, json);
    }
    #endregion

    #region Inven �� EquipInven �߰� / ���� / ���� ���� ���
    // "invendata.json"���� ����� ������ ������ �ҷ�����.
    public IEnumerator LoadToInvenData()
    {
        yield return new WaitForSecondsRealtime(1.0f);

        // InvenGridManager���� slotGrid �����ͼ� Dictionary�� ����.
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

            // �ҷ��� ������ ���
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
        if(toInvenItemDatas.Count <= 0)
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


        //for(int i = 0; i < InvenButtonList.Count; i++)
        //{
        //    // ������ Btn ��� �����ϱ�
        //    ItemButtonScript ibs = InvenButtonList[i].transform.GetComponent<ItemButtonScript>();
        //    // Spawn�ؼ� ���� ���ٴϴ� ����
        //    ibs.SpawnStoredItem();
        //
        //    //�޾ƿ� Grid �κ� string ����
        //    // �� : �κ�
        //    //string gridTypeKey = "Inven";
        //    // ����� Start Grid Position ����.
        //    //int gridX = 0;
        //    //int gridY = 0;
        //
        //    // ������ ���� ��������.
        //    // public List<string> invenPosData�� ����.
        //    InvenItemsPositionDataToJson();
        //
        //    string[] arr = invenPosData[i].Split("/");
        //
        //    // InvenDataPostioning(int x, int y) ������Ѽ� ��ġ��Ű��.
        //    IGMs[arr[0]].InvenDataPostioning(int.Parse(arr[1]), int.Parse(arr[2]));
        //}
    }

    // invenPositionJsonFileName���� ��ġ�� �������� �޼ҵ�
    public void InvenItemsPositionDataToJson()
    {
        // JSON ������ Resources �������� �б�
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(invenPositionJsonFileName));
        if (jsonTextAsset != null)
        {
            // JSON �����͸� GameData ��ü�� ��ȯ
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
        foreach(var data in totalInvenData)
        {
            GameObject invenButton = data.Value.Item1;
            string[] invenArr = data.Value.Item2.Split("/");

            // ������ Btn ��� �����ϱ�
            ItemButtonScript ibs = invenButton.transform.GetComponent<ItemButtonScript>();
            // Spawn�ؼ� ���� ���ٴϴ� ����
            ibs.SpawnStoredItem();

            IGMs[invenArr[0]].InvenDataPostioning(int.Parse(invenArr[1]), int.Parse(invenArr[2]));
        }
    }
    #endregion
}
