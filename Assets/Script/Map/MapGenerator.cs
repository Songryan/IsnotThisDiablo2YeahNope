using System.Collections.Generic;
using UnityEngine;

public class MapGenerator: MonoBehaviour
{
    public List<GameObject> roomPrefabs; // 방 Prefab들을 Inspector에서 할당하세요.
    public Vector3 startPosition; // 시작 위치
    public float roomOffset = 10.0f; // 방 간의 거리 (적절히 조정 필요)
    public int maxRooms = 10; // 최대 방 개수 (초기값 10)

    private Dictionary<GameObject, List<Transform>> roomPortals = new Dictionary<GameObject, List<Transform>>();
    private List<GameObject> generatedRooms = new List<GameObject>(); // 생성된 방들을 저장

    void Start()
    {
        InitializeAndGenerateRooms();
    }

    void InitializeAndGenerateRooms()
    {
        while (generatedRooms.Count < maxRooms)
        {
            InitializeRoomPortals();
            GenerateInitRooms();

            if (generatedRooms.Count < maxRooms)
            {
                //Debug.Log("Not enough rooms generated. Restarting...");
                ClearGeneratedRooms();
            }
        }
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
            //Debug.LogWarning($"Room {room.name} does not have a 'Portals' object.");
        }

        return portals;
    }

    void GenerateInitRooms()
    {
        GameObject initialRoomPrefab = roomPrefabs[2];
        GameObject initialRoom = Instantiate(initialRoomPrefab, startPosition, Quaternion.identity);
        initialRoom.AddComponent<BoxCollider>().isTrigger = true; // 트리거 Collider 추가

        generatedRooms.Add(initialRoom);

        List<Transform> initialRoomPortals = roomPortals[initialRoomPrefab];

        GenerateRooms(initialRoom, initialRoomPortals);
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
                //Debug.Log($"Collision Detected between new room and existing room {existingRoom.name}");
                return true;
            }
        }
        return false;
    }

    bool MatchPortalPositionAndRotation(Transform portalA, Transform portalB, GameObject currentRoom, GameObject newRoom)
    {
        // 포탈 A와 포탈 B의 월드 포지션을 계산
        Vector3 portalAWorldPosition = currentRoom.transform.TransformPoint(portalA.localPosition);
        Vector3 portalBWorldPosition = newRoom.transform.TransformPoint(portalB.localPosition);

        // 포탈 B의 위치를 포탈 A의 위치로 맞추기 위해 오프셋을 계산
        Vector3 positionOffset = portalAWorldPosition - portalBWorldPosition;
        newRoom.transform.position += positionOffset;

        // 새로운 포탈 B의 월드 포지션을 계산
        portalBWorldPosition = newRoom.transform.TransformPoint(portalB.localPosition);

        // 포탈 A와 포탈 B의 위치가 일치하는지 확인
        if (Vector3.Distance(portalAWorldPosition, portalBWorldPosition) < 0.01f)
        {
            for (int i = 0; i < 4; i++)
            {
                newRoom.transform.rotation = Quaternion.Euler(0, i * 90, 0);

                // 회전 후 포탈 B의 월드 포지션을 다시 계산
                portalBWorldPosition = newRoom.transform.TransformPoint(portalB.localPosition);
                positionOffset = portalAWorldPosition - portalBWorldPosition;

                // 위치가 일치하지 않으면 다시 조정
                if (Vector3.Distance(portalAWorldPosition, portalBWorldPosition) >= 0.01f)
                {
                    newRoom.transform.position += positionOffset;
                    portalBWorldPosition = newRoom.transform.TransformPoint(portalB.localPosition);
                }

                // 최종 위치와 회전 후 충돌 확인
                if (Vector3.Distance(portalAWorldPosition, portalBWorldPosition) < 0.01f && !CheckCollision(newRoom))
                {
                    return true; // 일치하고 충돌이 없으면 true 반환
                }
            }
        }

        return false; // 모든 회전에서 일치하지 않거나 충돌이 있으면 false 반환
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

                GameObject newRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
                //GameObject newRoomPrefab = roomPrefabs[0];
                GameObject newRoom = Instantiate(newRoomPrefab, Vector3.zero, Quaternion.identity);
                newRoom.AddComponent<BoxCollider>().isTrigger = true; // 트리거 Collider 추가

                Transform newRoomPortalsParent = newRoom.transform.Find("Portals");
                if (newRoomPortalsParent != null && newRoomPortalsParent.childCount > 0)
                {
                    // 자식 포탈 중 랜덤하게 선택
                    //int randomIndex = Random.Range(0, newRoomPortalsParent.childCount);
                    Transform newRoomPortal = newRoomPortalsParent.GetChild(0);

                    if (MatchPortalPositionAndRotation(portal, newRoomPortal, currentRoom, newRoom))
                    {
                        generatedRooms.Add(newRoom);

                        List<Transform> newRoomPortals = roomPortals[newRoomPrefab];
                        newRoomPortals.Remove(newRoomPortal);
                        roomsToProcess.Enqueue((newRoom, newRoomPortals));
                    }
                    else
                    {
                        Destroy(newRoom);
                    }
                }
            }
        }
    }

    void ClearGeneratedRooms()
    {
        foreach (var room in generatedRooms)
        {
            Destroy(room);
        }
        generatedRooms.Clear();
        roomPortals.Clear();
    }
}
