using System.Collections.Generic;
using UnityEngine;

public class ProceduralRoomGenerator : MonoBehaviour
{
    public List<GameObject> roomPrefabs; // �� Prefab���� Inspector���� �Ҵ��ϼ���.
    public Vector3 startPosition; // ���� ��ġ
    public float roomOffset = 10.0f; // �� ���� �Ÿ� (������ ���� �ʿ�)

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
        // �������� �ϳ��� �� ����
        GameObject initialRoomPrefab = roomPrefabs[2];
        GameObject initialRoom = Instantiate(initialRoomPrefab, startPosition, Quaternion.identity);

        List<Transform> initialRoomPortals = roomPortals[initialRoomPrefab];

        foreach (Transform portal in initialRoomPortals)
        {
            // ���ο� ���� �������� ����
            GameObject newRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
            GameObject newRoom = Instantiate(newRoomPrefab, Vector3.zero, Quaternion.identity);

            // ���� ��Ż�� ���߱� ���� ȸ���� ��ġ ����
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
        // 1. ��Ż A�� ���� ȸ���� �ݴ밡 �ǵ��� ��Ż B�� ȸ���� ����
        newRoom.transform.rotation = Quaternion.Euler(0, 180, 0);

        // 2. ��Ż B�� ���� �������� ��Ż A�� ���� �����ǰ� ��ġ��Ű�� ���� ��ġ�� ����
        Vector3 positionOffset = portalA.position - portalB.position;
        newRoom.transform.position += positionOffset;

        // ��Ż�� ��Ȯ�� ��ġ���� Ȯ���Ͽ� �̼� ����
        positionOffset = portalA.position - portalB.position;
        newRoom.transform.position += positionOffset;
    }
}
