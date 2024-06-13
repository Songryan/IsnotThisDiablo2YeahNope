using System.Collections.Generic;
using UnityEngine;

public class RoomInfo
{
    public GameObject Prefab;   // 방의 프리팹
    public Vector2 Size;        // 방의 크기
    public List<Vector2> Doors; // 문의 위치 리스트

    public RoomInfo(GameObject prefab, Vector2 size, List<Vector2> doors)
    {
        Prefab = prefab;
        Size = size;
        Doors = doors;
    }

    // 방이 회전할 때 문의 위치를 계산하는 메서드
    public List<Vector2> GetRotatedDoors(int rotation)
    {
        List<Vector2> rotatedDoors = new List<Vector2>();
        foreach (var door in Doors)
        {
            Vector2 rotatedDoor = door;
            switch (rotation)
            {
                case 1: // 90도 회전
                    rotatedDoor = new Vector2(door.y, Size.x - door.x);
                    break;
                case 2: // 180도 회전
                    rotatedDoor = new Vector2(Size.x - door.x, Size.y - door.y);
                    break;
                case 3: // 270도 회전
                    rotatedDoor = new Vector2(Size.y - door.y, door.x);
                    break;
                default: // 0도 회전
                    rotatedDoor = door;
                    break;
            }
            rotatedDoors.Add(rotatedDoor);
        }
        return rotatedDoors;
    }
}
