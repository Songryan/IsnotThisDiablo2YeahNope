using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Cinemachine;

public class GamePlayManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Minotaur_Legacy;
    [Header("Camera")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private static GamePlayManager _instance;
    public static GamePlayManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GamePlayManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(GamePlayManager).ToString());
                    _instance = singletonObject.AddComponent<GamePlayManager>();
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

    private void Start()
    {
        // 맵 굽기
        BakedMapNavMesh();
        // 플레이어 생성
        GeneratePlayer();
        // 몬스터 생성
        GenerateMonsters();
        // UI Refresh
        InGameUIManager.Instance.RefreshUI();
    }

    void BakedMapNavMesh()
    {
        GameObject mapParent = GameObject.Find("MapParent");
        if (mapParent != null)
        {
            NavMeshSurface navMeshSurface = mapParent.AddComponent<NavMeshSurface>();
            navMeshSurface.BuildNavMesh();
        }
    }

    void GeneratePlayer()
    {
        // 첫번째 방에 플레이어 생성.
        Transform StartingRoom = GameObject.Find("StartingRoom").transform.Find("StartOREndPosition").transform;

        GameObject pala = Instantiate(Player, StartingRoom.transform.position, Quaternion.identity);

        virtualCamera.Follow = pala.transform;
        virtualCamera.LookAt = pala.transform;
    }

    void GenerateMonsters()
    {
        // 각 방의 
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("StartEndPosition");

        Transform[] wayWaypoints = new Transform[spawnPointObjects.Length];

        for(int i = 0; i < spawnPointObjects.Length; i++)
        {
            wayWaypoints[i] = spawnPointObjects[i].transform;
        }

        // Patrol지역 무작위로 섞기
        ShuffleArray(wayWaypoints);
        ShuffleArray(spawnPointObjects);

        // 랜덤 인스턴스 생성
        System.Random random = new System.Random();

        foreach (var point in spawnPointObjects)
        {
            // 33% 확률로 몬스터 생성
            if (random.NextDouble() <= 0.33)
            {
                GameObject Minotaur = Instantiate(Minotaur_Legacy, point.transform.position, Quaternion.identity);
                Minotaur.GetComponent<MonsterBT>().waypoints = wayWaypoints;
            }
        }
    }

    // Fisher-Yates 셔플 알고리즘을 사용하여 배열을 무작위로 섞는 제네릭 메서드
    void ShuffleArray<T>(T[] array)
    {
        System.Random random = new System.Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
