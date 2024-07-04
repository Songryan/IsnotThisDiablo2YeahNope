using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public List<GameObject> roomPrefabs;
    public Vector3 startPosition;
    public int maxRooms = 10;

    [SerializeField] Mesh ClosedPortalMesh; // 변경에 사용할 Mesh 필드 변수
    [SerializeField] Mesh OpenPortalMesh; // 변경에 사용할 Mesh 필드 변수

    [SerializeField] GameObject EndPointObj; // 변경에 사용할 Mesh 필드 변수
    [SerializeField] GameObject Player; // 변경에 사용할 Mesh 필드 변수

    private Dictionary<GameObject, List<Transform>> roomPortals = new Dictionary<GameObject, List<Transform>>();
    public List<GameObject> GeneratedRooms { get; private set; } = new List<GameObject>();

    public float Progress { get; private set; } // 외부에서 접근할 수 있도록 프로퍼티 추가

    Dictionary<GameObject, Vector3> portalPositions = new Dictionary<GameObject, Vector3>();

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public IEnumerator InitializeAndGenerateRoomsAsync()
    {
        Progress = 0;
        while (GeneratedRooms.Count < maxRooms)
        {
            InitializeRoomPortals();
            yield return StartCoroutine(GenerateInitRoomsAsync());

            if (GeneratedRooms.Count < maxRooms)
            {
                ClearGeneratedRooms();
            }
        }

        // Room에서 BoxCollider 삭제하는 기능
        RemoveBoxCollider();

        // 첫번째 방 StartOREndPosition에 Player 생성
        //GeneratePlayer();

        // 마지막 방 StartOREndPosition에 오브젝트 생성
        GenerateEndObject();

        // 포탈을 찾아서 위치를 저장하는 기능 호출
        FindAndStorePortalPositions();

        // parent Object로 이동
        MoveGeneratedRoomsToBattleScene();

        Progress = 1.0f; // 완료 시 진행도를 100%로 설정
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
        return portals;
    }

    IEnumerator GenerateInitRoomsAsync()
    {
        GameObject initialRoomPrefab = roomPrefabs[2];
        GameObject initialRoom = Instantiate(initialRoomPrefab, startPosition, Quaternion.identity);
        initialRoom.name = "StartingRoom";
        initialRoom.AddComponent<BoxCollider>().isTrigger = true;

        GeneratedRooms.Add(initialRoom);

        List<Transform> initialRoomPortals = roomPortals[initialRoomPrefab];

        yield return StartCoroutine(GenerateRoomsAsync(initialRoom, initialRoomPortals));
    }

    bool CheckCollision(GameObject newRoom)
    {
        BoxCollider newCollider = newRoom.GetComponent<BoxCollider>();
        if (newCollider == null) return false;

        foreach (var existingRoom in GeneratedRooms)
        {
            BoxCollider existingCollider = existingRoom.GetComponent<BoxCollider>();
            if (existingCollider == null) continue;

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
                return true;
            }
        }
        return false;
    }

    bool MatchPortalPositionAndRotation(Transform portalA, Transform portalB, GameObject currentRoom, GameObject newRoom)
    {
        Vector3 portalAWorldPosition = currentRoom.transform.TransformPoint(portalA.localPosition);
        Vector3 portalBWorldPosition = newRoom.transform.TransformPoint(portalB.localPosition);

        Vector3 positionOffset = portalAWorldPosition - portalBWorldPosition;
        newRoom.transform.position += positionOffset;

        portalBWorldPosition = newRoom.transform.TransformPoint(portalB.localPosition);

        if (Vector3.Distance(portalAWorldPosition, portalBWorldPosition) < 0.01f)
        {
            for (int i = 0; i < 4; i++)
            {
                newRoom.transform.rotation = Quaternion.Euler(0, i * 90, 0);

                portalBWorldPosition = newRoom.transform.TransformPoint(portalB.localPosition);
                positionOffset = portalAWorldPosition - portalBWorldPosition;

                if (Vector3.Distance(portalAWorldPosition, portalBWorldPosition) >= 0.01f)
                {
                    newRoom.transform.position += positionOffset;
                    portalBWorldPosition = newRoom.transform.TransformPoint(portalB.localPosition);
                }

                if (Vector3.Distance(portalAWorldPosition, portalBWorldPosition) < 0.01f && !CheckCollision(newRoom))
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator GenerateRoomsAsync(GameObject initialRoom, List<Transform> initialRoomPortals)
    {
        Queue<(GameObject, List<Transform>)> roomsToProcess = new Queue<(GameObject, List<Transform>)>();
        roomsToProcess.Enqueue((initialRoom, initialRoomPortals));

        while (roomsToProcess.Count > 0 && GeneratedRooms.Count < maxRooms)
        {
            var (currentRoom, currentRoomPortals) = roomsToProcess.Dequeue();

            foreach (Transform portal in currentRoomPortals)
            {
                if (GeneratedRooms.Count >= maxRooms) break;

                GameObject newRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
                GameObject newRoom = Instantiate(newRoomPrefab, Vector3.zero, Quaternion.identity);
                newRoom.AddComponent<BoxCollider>().isTrigger = true;

                Transform newRoomPortalsParent = newRoom.transform.Find("Portals");
                if (newRoomPortalsParent != null && newRoomPortalsParent.childCount > 0)
                {
                    Transform newRoomPortal = newRoomPortalsParent.GetChild(0);

                    if (MatchPortalPositionAndRotation(portal, newRoomPortal, currentRoom, newRoom))
                    {
                        GeneratedRooms.Add(newRoom);

                        List<Transform> newRoomPortals = roomPortals[newRoomPrefab];
                        newRoomPortals.Remove(newRoomPortal);
                        roomsToProcess.Enqueue((newRoom, newRoomPortals));
                    }
                    else
                    {
                        Destroy(newRoom);
                    }
                }

                Progress = (float)GeneratedRooms.Count / maxRooms;
                yield return null;
            }
        }
    }

    void ClearGeneratedRooms()
    {
        foreach (var room in GeneratedRooms)
        {
            Destroy(room);
        }
        GeneratedRooms.Clear();
        roomPortals.Clear();
    }

    // 포탈의 메쉬를 변경하는 기능을 따로 메서드로 구현
    void UpdatePortalMesh(Transform portal)
    {
        MeshFilter meshFilter = portal.GetComponent<MeshFilter>();
        MeshCollider meshCollider = portal.GetComponent<MeshCollider>();

        if (meshFilter != null)
        {
            meshFilter.sharedMesh = ClosedPortalMesh; // 새로운 메쉬로 변경
        }

        if (meshCollider != null)
        {
            meshCollider.sharedMesh = ClosedPortalMesh; // 새로운 메쉬로 변경
        }
    }

    void UpdatePortalMesh2(Transform portal)
    {
        MeshFilter meshFilter = portal.GetComponent<MeshFilter>();
        MeshCollider meshCollider = portal.GetComponent<MeshCollider>();

        if (meshFilter != null)
        {
            meshFilter.sharedMesh = OpenPortalMesh; // 새로운 메쉬로 변경
        }

        if (meshCollider != null)
        {
            meshCollider.sharedMesh = OpenPortalMesh; // 새로운 메쉬로 변경
        }
    }

    void FindAndStorePortalPositions()
    {
        portalPositions.Clear(); // 기존 데이터를 초기화

        foreach (var room in GeneratedRooms)
        {
            Transform portalsTransform = room.transform.Find("Portals");
            if (portalsTransform != null)
            {
                foreach (Transform portal in portalsTransform)
                {
                    UpdatePortalMesh2(portal.transform);
                    portalPositions.Add(portal.gameObject, portal.position); // 자식 오브젝트와 월드 포지션 저장
                }
            }
        }
        // 중복된 위치를 가진 포탈 제거
        RemoveDuplicatePortals();
    }

    void RemoveDuplicatePortals()
    {
        var keysToRemove = new List<GameObject>();

        // 이중 foreach 문으로 중복된 위치 찾기 및 삭제
        foreach (var kvp1 in portalPositions)
        {
            foreach (var kvp2 in portalPositions)
            {
                if (kvp1.Key != kvp2.Key && kvp1.Value == kvp2.Value)
                {
                    // 중복된 위치를 가진 키를 저장
                    if (!keysToRemove.Contains(kvp1.Key))
                    {
                        keysToRemove.Add(kvp1.Key);
                    }
                    if (!keysToRemove.Contains(kvp2.Key))
                    {
                        keysToRemove.Add(kvp2.Key);
                    }
                }
            }
        }

        // 중복된 키-값 쌍을 제거
        foreach (var key in keysToRemove)
        {
            portalPositions.Remove(key);
        }

        foreach (var Obj in portalPositions)
        {
            UpdatePortalMesh(Obj.Key.transform);
        }
    }

    void RemoveBoxCollider()
    {
        foreach (var room in GeneratedRooms)
        {
            BoxCollider[] colliders = room.GetComponents<BoxCollider>();
            foreach (BoxCollider collider in colliders)
            {
                Destroy(collider);
            }
        }
    }

    void GeneratePlayer()
    {
        // 첫번째 방에 플레이어 생성.
        Transform position = GeneratedRooms[0].transform.Find("StartOREndPosition");
        GameObject pala = Instantiate(Player, position.position, position.rotation);
    }

    void GenerateEndObject()
    {
        // 맨 마지막 방에 생성.
        GameObject position = GeneratedRooms[GeneratedRooms.Count - 1].transform.Find("StartOREndPosition").gameObject;

        GameObject endObject = Instantiate(EndPointObj, position.transform.position, position.transform.rotation);
        endObject.transform.SetParent(position.transform);
    }

    private void MoveGeneratedRoomsToBattleScene()
    {
        GameObject mapParent = new GameObject("MapParent");

        foreach (var room in GeneratedRooms)
        {
            room.transform.SetParent(mapParent.transform);
        }
    }
}

