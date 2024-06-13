using UnityEngine;
using System.Collections.Generic;

public class RoomComponent : MonoBehaviour
{
    // 문 위치를 저장하는 리스트
    public List<Transform> Doors;

    // 문 위치 캐싱을 위한 static 딕셔너리
    private static Dictionary<string, List<Vector2>> doorPositionsCache = new Dictionary<string, List<Vector2>>();

    // 방 크기 캐싱을 위한 static 딕셔너리
    private static Dictionary<string, Vector2> roomSizeCache = new Dictionary<string, Vector2>();

    void Start()
    {
        // 문 위치를 수집하기 위해 리스트 초기화
        Doors = new List<Transform>();

        // 'Portals'라는 자식 트랜스폼을 찾음
        Transform portals = transform.Find("Portals");

        // 'Portals' 자식 트랜스폼이 존재하면
        if (portals != null)
        {
            // 'Portals' 자식 오브젝트들을 순회
            foreach (Transform child in portals)
            {
                // 문 위치를 리스트에 추가
                Doors.Add(child);
            }
        }
    }

    // 문 위치를 로컬 좌표로 반환하는 메서드
    public List<Vector2> GetLocalDoorPositions()
    {
        // 프리팹 이름을 가져옴
        string prefabName = GetPrefabName();

        // 캐시에 문 위치가 저장되어 있으면 반환
        if (doorPositionsCache.TryGetValue(prefabName, out List<Vector2> cachedDoorPositions))
        {
            return cachedDoorPositions;
        }

        // 캐시에 문 위치가 없으면 계산
        List<Vector2> doorPositions = new List<Vector2>();
        foreach (Transform door in Doors)
        {
            // 문 위치를 로컬 좌표로 변환
            Vector3 localPos = transform.InverseTransformPoint(door.position);
            doorPositions.Add(new Vector2(localPos.x, localPos.z));
        }

        // 계산된 문 위치를 캐시에 저장
        doorPositionsCache[prefabName] = doorPositions;
        return doorPositions;
    }

    // 방 외벽의 크기를 반환하는 메서드
    public Vector2 GetRoomExternalSize()
    {
        // 프리팹 이름을 가져옴
        string prefabName = GetPrefabName();

        // 캐시에 방 크기가 저장되어 있으면 반환
        if (roomSizeCache.TryGetValue(prefabName, out Vector2 cachedSize))
        {
            return cachedSize;
        }

        // 'Corners - External' 자식 트랜스폼을 찾음
        Transform cornersExternal = transform.Find("Corners - External");

        // 'Corners - External'이 없으면 경고 메시지 출력 후 크기 0 반환
        if (cornersExternal == null)
        {
            Debug.LogWarning("Corners - External not found in " + gameObject.name);
            return Vector2.zero;
        }

        // 'Corners - External' 자식 오브젝트들에서 Renderer 컴포넌트를 찾음
        Renderer[] renderers = cornersExternal.GetComponentsInChildren<Renderer>();

        // Renderer가 없으면 경고 메시지 출력 후 크기 0 반환
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found in Corners - External of " + gameObject.name);
            return Vector2.zero;
        }

        // 첫 번째 Renderer의 바운드를 초기 바운드로 설정
        Bounds bounds = renderers[0].bounds;

        // 나머지 Renderer의 바운드를 포함하도록 확장
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        // 최종 바운드의 크기를 계산하여 저장
        Vector3 size = bounds.size;
        Vector2 roomSize = new Vector2(size.x, size.z);

        // 계산된 방 크기를 캐시에 저장
        roomSizeCache[prefabName] = roomSize;
        return roomSize;
    }

    // 프리팹 이름을 가져오는 메서드
    private string GetPrefabName()
    {
        return gameObject.name;
    }
}
