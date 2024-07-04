using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Cinemachine;

public class GamePlayManager : MonoBehaviour
{
    [SerializeField] GameObject Player;

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

        GeneratePlayer();
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
}
