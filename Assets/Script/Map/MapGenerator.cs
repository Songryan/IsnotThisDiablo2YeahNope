using System.Collections.Generic;
using UnityEngine;

public class ProceduralRoomGenerator : MonoBehaviour
{
    public List<GameObject> roomPrefabs; // 방 Prefab들을 Inspector에서 할당하세요.
    public Vector3 startPosition; // 시작 위치
    public float roomOffset = 10.0f; // 방 간의 거리 (적절히 조정 필요)
    public int maxRooms = 10; // 최대 방 개수 (초기값 10)

    private Dictionary<GameObject, List<Transform>> roomPortals = new Dictionary<GameObject, List<Transform>>();
    private List<GameObject> generatedRooms = new List<GameObject>(); // 생성된 방들을 저장

    void Start()
    {
        InitializeRoomPortals();
        GenerateInitRooms();
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

    void GenerateInitRooms()
    {
        GameObject initialRoomPrefab = roomPrefabs[1];
        GameObject initialRoom = Instantiate(initialRoomPrefab, startPosition, Quaternion.identity);
        initialRoom.AddComponent<BoxCollider>().isTrigger = true; // 트리거 Collider 추가

        generatedRooms.Add(initialRoom);

        List<Transform> initialRoomPortals = roomPortals[initialRoomPrefab];

        GenerateRooms(initialRoom, initialRoomPortals);
    }

    void MatchPortalPositionAndRotation(Transform portalA, Transform portalB, GameObject newRoom)
    {
        // 포탈 B의 월드 포지션을 포탈 A의 월드 포지션과 일치시키기 위해 위치를 조정
        Vector3 positionOffset = portalA.position - portalB.position;
        newRoom.transform.position = newRoom.transform.position + positionOffset;
    }


    bool CheckCollision(GameObject newRoom)
    {
        BoxCollider newCollider = newRoom.GetComponent<BoxCollider>();
        if (newCollider == null)
        {
            return false;
        }

        foreach (var existingRoom in generatedRooms)
        {
            BoxCollider existingCollider = existingRoom.GetComponent<BoxCollider>();
            if (existingCollider == null)
            {
                continue;
            }

            Vector3 worldCenterNew = newCollider.transform.TransformPoint(newCollider.center);
            Vector3 worldCenterExisting = existingCollider.transform.TransformPoint(existingCollider.center);
            Vector3 worldSizeNew = newCollider.transform.TransformVector(newCollider.size);
            Vector3 worldSizeExisting = existingCollider.transform.TransformVector(existingCollider.size);

            worldSizeNew = new Vector3(Mathf.Abs(worldSizeNew.x), Mathf.Abs(worldSizeNew.y), Mathf.Abs(worldSizeNew.z));
            worldSizeExisting = new Vector3(Mathf.Abs(worldSizeExisting.x), Mathf.Abs(worldSizeExisting.y), Mathf.Abs(worldSizeExisting.z));

            Bounds boundsNew = new Bounds(worldCenterNew, worldSizeNew);
            Bounds boundsExisting = new Bounds(worldCenterExisting, worldSizeExisting);

            if (boundsNew.Intersects(boundsExisting))
            {
                Debug.Log($"Collision Detected between new room and existing room {existingRoom.name}");
                return true;
            }
        }
        return false;
    }

    void GenerateRooms(GameObject initialRoom, List<Transform> initialRoomPortals)
    {
        Queue<(GameObject, List<Transform>)> roomsToProcess = new Queue<(GameObject, List<Transform>)>();
        roomsToProcess.Enqueue((initialRoom, initialRoomPortals));

        while (roomsToProcess.Count > 0 && generatedRooms.Count < maxRooms)
        {
            var (currentRoom, currentRoomPortals) = roomsToProcess.Dequeue();

            foreach (Transform portal in currentRoomPortals)
            {
                if (generatedRooms.Count >= maxRooms)
                {
                    break;
                }

                GameObject newRoomPrefab = roomPrefabs[0];
                //GameObject newRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
                GameObject newRoom = Instantiate(newRoomPrefab, Vector3.zero, Quaternion.identity);
                newRoom.AddComponent<BoxCollider>().isTrigger = true; // 트리거 Collider 추가

                Transform newRoomPortalsParent = newRoom.transform.Find("Portals");
                if (newRoomPortalsParent != null && newRoomPortalsParent.childCount > 0)
                {
                    // 자식 포탈 중 랜덤하게 선택
                    //int randomIndex = Random.Range(0, newRoomPortalsParent.childCount);
                    Transform newRoomPortal = newRoomPortalsParent.GetChild(0);

                    bool isCollision = true;
                    for (int i = 0; i < 4 && isCollision; i++)
                    {
                        MatchPortalPositionAndRotation(portal, newRoomPortal, newRoom);
                        isCollision = CheckCollision(newRoom);

                        if (isCollision)
                        {
                            newRoom.transform.rotation = Quaternion.Euler(0, (i + 1) * 90, 0);
                        }
                    }

                    if (isCollision)
                    {
                        Destroy(newRoom);
                        continue;
                    }

                    generatedRooms.Add(newRoom);

                    List<Transform> newRoomPortals = roomPortals[newRoomPrefab];
                    newRoomPortals.Remove(newRoomPortal);
                    roomsToProcess.Enqueue((newRoom, newRoomPortals));
                }
            }
        }
    }

}
