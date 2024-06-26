using System.IO;
using UnityEngine;

public class JsonDataManager : MonoBehaviour
{
    [SerializeField] private string jsonFileName = "gamedata.json";
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
                Debug.Log($"GlobalID: {entry.GlobalID}, Level: {entry.Level}, Quality: {entry.Quality}, StatBonus: {entry.StatBonus}");
            }
        }
        else
        {
            Debug.LogWarning("JSON file not found in Resources");
        }

        // JSON 파일 경로 설정 (안드로이드에서는 persistentDataPath 사용)
        jsonFilePath = Path.Combine(Application.persistentDataPath, jsonFileName);

        // 예제 데이터 생성
        GameData gameData = CreateSampleData();

        // JSON으로 저장
        SaveToJson(gameData, jsonFilePath);

        // JSON에서 불러오기
        GameData loadedJsonData = LoadFromJson(jsonFilePath);

        // 불러온 데이터 출력 (확인용)
        foreach (var entry in loadedJsonData.Entries)
        {
            Debug.Log($"GlobalID: {entry.GlobalID}, Level: {entry.Level}, Quality: {entry.Quality}, StatBonus: {entry.StatBonus}");
        }
    }

    public GameData CreateSampleData()
    {
        GameData gameData = new GameData();
        gameData.Entries.Add(new GameDataEntry(0, 1, 0, "0/0/0/0"));
        gameData.Entries.Add(new GameDataEntry(0, 1, 1, "1/0/0/0"));
        gameData.Entries.Add(new GameDataEntry(0, 1, 2, "0/1/1/0"));
        gameData.Entries.Add(new GameDataEntry(0, 1, 3, "1/1/0/1"));

        return gameData;
    }

    public void SaveToJson(GameData gameData, string filePath)
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(filePath, json);
    }

    public GameData LoadFromJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found");
            return null;
        }
    }
}