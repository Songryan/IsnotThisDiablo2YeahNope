using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public int dungeonWidth = 50;
    public int dungeonHeight = 50;
    public int minRoomSize = 6;
    public int maxRoomSize = 12;
    public int maxNodes = 5;

    private List<RoomInfo> roomInfos;
    private List<Rect> rooms;
    private List<Rect> corridors;

    void Start()
    {
        roomInfos = new List<RoomInfo>();

        // 방 프리팹의 정보를 자동으로 등록합니다.
        foreach (var prefab in roomPrefabs)
        {
            RoomComponent roomComponent = prefab.GetComponent<RoomComponent>();
            if (roomComponent != null)
            {
                Vector2 size = roomComponent.GetRoomExternalSize();
                if (size != Vector2.zero)
                {
                    List<Vector2> doors = roomComponent.GetLocalDoorPositions();
                    roomInfos.Add(new RoomInfo(prefab, size, doors));
                }
            }
        }

        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        rooms = new List<Rect>();
        corridors = new List<Rect>();
        SplitSpace(new Rect(0, 0, dungeonWidth, dungeonHeight), maxNodes);

        // 각 분할된 공간에 방을 배치합니다.
        foreach (var roomRect in rooms)
        {
            PlaceRoom(roomRect);
        }

        // 방과 방 사이를 연결합니다.
        for (int i = 1; i < rooms.Count; i++)
        {
            ConnectRooms(rooms[i - 1], rooms[i]);
        }
    }

    void SplitSpace(Rect space, int nodesLeft)
    {
        if (nodesLeft == 0 || (space.width <= maxRoomSize && space.height <= maxRoomSize))
        {
            rooms.Add(space);
            return;
        }

        bool splitHorizontally = Random.Range(0, 2) == 0;

        if (splitHorizontally && space.height > 2 * minRoomSize)
        {
            float split = Random.Range(minRoomSize, space.height - minRoomSize);
            SplitSpace(new Rect(space.x, space.y, space.width, split), nodesLeft - 1);
            SplitSpace(new Rect(space.x, space.y + split, space.width, space.height - split), nodesLeft - 1);
        }
        else if (!splitHorizontally && space.width > 2 * minRoomSize)
        {
            float split = Random.Range(minRoomSize, space.width - minRoomSize);
            SplitSpace(new Rect(space.x, space.y, split, space.height), nodesLeft - 1);
            SplitSpace(new Rect(space.x + split, space.y, space.width - split, space.height), nodesLeft - 1);
        }
        else
        {
            rooms.Add(space);
        }
    }

    void PlaceRoom(Rect roomRect)
    {
        RoomInfo roomInfo = roomInfos[Random.Range(0, roomInfos.Count)];
        Vector2 roomSize = roomInfo.Size;
        Vector2 roomPosition = new Vector2(
            roomRect.x + (roomRect.width - roomSize.x) / 2,
            roomRect.y + (roomRect.height - roomSize.y) / 2
        );

        Instantiate(roomInfo.Prefab, new Vector3(roomPosition.x, 0, roomPosition.y), Quaternion.identity);
    }

    void ConnectRooms(Rect roomA, Rect roomB)
    {
        Vector2 pointA = new Vector2(roomA.x + roomA.width / 2, roomA.y + roomA.height / 2);
        Vector2 pointB = new Vector2(roomB.x + roomB.width / 2, roomB.y + roomB.height / 2);

        if (Random.Range(0, 2) == 0)
        {
            CreateCorridorLine(pointA, new Vector2(pointB.x, pointA.y));
            CreateCorridorLine(new Vector2(pointB.x, pointA.y), pointB);
        }
        else
        {
            CreateCorridorLine(pointA, new Vector2(pointA.x, pointB.y));
            CreateCorridorLine(new Vector2(pointA.x, pointB.y), pointB);
        }
    }

    void CreateCorridorLine(Vector2 start, Vector2 end)
    {
        Vector2 current = start;
        while (current != end)
        {
            if (current.x != end.x)
            {
                current.x += (end.x > current.x) ? 1 : -1;
            }
            else if (current.y != end.y)
            {
                current.y += (end.y > current.y) ? 1 : -1;
            }

            GameObject corridor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            corridor.transform.position = new Vector3(current.x, 0, current.y);
            corridor.GetComponent<Renderer>().material.color = Color.gray;
        }
    }
}
