using UnityEngine;
using System.Collections.Generic;

public class RoomComponent : MonoBehaviour
{
    // �� ��ġ�� �����ϴ� ����Ʈ
    public List<Transform> Doors;

    // �� ��ġ ĳ���� ���� static ��ųʸ�
    private static Dictionary<string, List<Vector2>> doorPositionsCache = new Dictionary<string, List<Vector2>>();

    // �� ũ�� ĳ���� ���� static ��ųʸ�
    private static Dictionary<string, Vector2> roomSizeCache = new Dictionary<string, Vector2>();

    void Start()
    {
        // �� ��ġ�� �����ϱ� ���� ����Ʈ �ʱ�ȭ
        Doors = new List<Transform>();

        // 'Portals'��� �ڽ� Ʈ�������� ã��
        Transform portals = transform.Find("Portals");

        // 'Portals' �ڽ� Ʈ�������� �����ϸ�
        if (portals != null)
        {
            // 'Portals' �ڽ� ������Ʈ���� ��ȸ
            foreach (Transform child in portals)
            {
                // �� ��ġ�� ����Ʈ�� �߰�
                Doors.Add(child);
            }
        }
    }

    // �� ��ġ�� ���� ��ǥ�� ��ȯ�ϴ� �޼���
    public List<Vector2> GetLocalDoorPositions()
    {
        // ������ �̸��� ������
        string prefabName = GetPrefabName();

        // ĳ�ÿ� �� ��ġ�� ����Ǿ� ������ ��ȯ
        if (doorPositionsCache.TryGetValue(prefabName, out List<Vector2> cachedDoorPositions))
        {
            return cachedDoorPositions;
        }

        // ĳ�ÿ� �� ��ġ�� ������ ���
        List<Vector2> doorPositions = new List<Vector2>();
        foreach (Transform door in Doors)
        {
            // �� ��ġ�� ���� ��ǥ�� ��ȯ
            Vector3 localPos = transform.InverseTransformPoint(door.position);
            doorPositions.Add(new Vector2(localPos.x, localPos.z));
        }

        // ���� �� ��ġ�� ĳ�ÿ� ����
        doorPositionsCache[prefabName] = doorPositions;
        return doorPositions;
    }

    // �� �ܺ��� ũ�⸦ ��ȯ�ϴ� �޼���
    public Vector2 GetRoomExternalSize()
    {
        // ������ �̸��� ������
        string prefabName = GetPrefabName();

        // ĳ�ÿ� �� ũ�Ⱑ ����Ǿ� ������ ��ȯ
        if (roomSizeCache.TryGetValue(prefabName, out Vector2 cachedSize))
        {
            return cachedSize;
        }

        // 'Corners - External' �ڽ� Ʈ�������� ã��
        Transform cornersExternal = transform.Find("Corners - External");

        // 'Corners - External'�� ������ ��� �޽��� ��� �� ũ�� 0 ��ȯ
        if (cornersExternal == null)
        {
            Debug.LogWarning("Corners - External not found in " + gameObject.name);
            return Vector2.zero;
        }

        // 'Corners - External' �ڽ� ������Ʈ�鿡�� Renderer ������Ʈ�� ã��
        Renderer[] renderers = cornersExternal.GetComponentsInChildren<Renderer>();

        // Renderer�� ������ ��� �޽��� ��� �� ũ�� 0 ��ȯ
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found in Corners - External of " + gameObject.name);
            return Vector2.zero;
        }

        // ù ��° Renderer�� �ٿ�带 �ʱ� �ٿ��� ����
        Bounds bounds = renderers[0].bounds;

        // ������ Renderer�� �ٿ�带 �����ϵ��� Ȯ��
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        // ���� �ٿ���� ũ�⸦ ����Ͽ� ����
        Vector3 size = bounds.size;
        Vector2 roomSize = new Vector2(size.x, size.z);

        // ���� �� ũ�⸦ ĳ�ÿ� ����
        roomSizeCache[prefabName] = roomSize;
        return roomSize;
    }

    // ������ �̸��� �������� �޼���
    private string GetPrefabName()
    {
        return gameObject.name;
    }
}
