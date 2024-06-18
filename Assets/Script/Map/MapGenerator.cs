using System.Collections.Generic;
using UnityEngine;

public class ProceduralRoomGenerator : MonoBehaviour
{
    public List<GameObject> roomPrefabs; // 방 Prefab들을 Inspector에서 할당하세요.
    public Vector3 startPosition; // 시작 위치
    public float roomOffset = 10.0f; // 방 간의 거리 (적절히 조정 필요)

    private Dictionary<GameObject, List<Transform>> roomPortals = new Dictionary<GameObject, List<Transform>>();

    void Start()
    {
        InitializeRoomPortals();
        GenerateRooms();
    }

    void InitializeRoomPortals()
    {
        foreach (var room in roomPrefabs)
        {
            List<Transform> portals = FindPortals(room);
            roomPortals.Add(room, portals);
        }
    }

    List<Transform> FindPortals(GameObject room)
    {
        List<Transform> portals = new List<Transform>();
        Transform portalsTransform = room.transform.Find("Portals");

        if (portalsTransform != null)
        {
            foreach (Transform portal in portalsTransform)
            {
                portals.Add(portal);
            }
        }
        else
        {
            Debug.LogWarning($"Room {room.name} does not have a 'Portals' object.");
        }

        return portals;
    }

    void GenerateRooms()
    {
        // 무작위로 하나의 방 선택
        GameObject initialRoomPrefab = roomPrefabs[2];
        GameObject initialRoom = Instantiate(initialRoomPrefab, startPosition, Quaternion.identity);

        List<Transform> initialRoomPortals = roomPortals[initialRoomPrefab];

        foreach (Transform portal in initialRoomPortals)
        {
            // 새로운 방을 무작위로 선택
            GameObject newRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
            GameObject newRoom = Instantiate(newRoomPrefab, Vector3.zero, Quaternion.identity);

            // 방의 포탈을 맞추기 위해 회전과 위치 조정
            Transform newRoomPortalsParent = newRoom.transform.Find("Portals");
            if (newRoomPortalsParent != null && newRoomPortalsParent.childCount > 0)
            {
                Transform newRoomPortal = newRoomPortalsParent.GetChild(0);
                MatchPortalPositionAndRotation(portal, newRoomPortal, newRoom);
            }
        }
    }

    void MatchPortalPositionAndRotation(Transform portalA, Transform portalB, GameObject newRoom)
    {
        // 1. 포탈 A의 월드 회전과 반대가 되도록 포탈 B의 회전을 조정
        newRoom.transform.rotation = Quaternion.Euler(0, 180, 0);

        // 2. 포탈 B의 월드 포지션을 포탈 A의 월드 포지션과 일치시키기 위해 위치를 조정
        Vector3 positionOffset = portalA.position - portalB.position;
        newRoom.transform.position += positionOffset;

        // 포탈이 정확히 겹치는지 확인하여 미세 조정
        positionOffset = portalA.position - portalB.position;
        newRoom.transform.position += positionOffset;
    }
}
