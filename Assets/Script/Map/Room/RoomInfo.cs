using System.Collections.Generic;
using UnityEngine;

public class RoomInfo
{
    public GameObject Prefab;   // ���� ������
    public Vector2 Size;        // ���� ũ��
    public List<Vector2> Doors; // ���� ��ġ ����Ʈ

    public RoomInfo(GameObject prefab, Vector2 size, List<Vector2> doors)
    {
        Prefab = prefab;
        Size = size;
        Doors = doors;
    }

    // ���� ȸ���� �� ���� ��ġ�� ����ϴ� �޼���
    public List<Vector2> GetRotatedDoors(int rotation)
    {
        List<Vector2> rotatedDoors = new List<Vector2>();
        foreach (var door in Doors)
        {
            Vector2 rotatedDoor = door;
            switch (rotation)
            {
                case 1: // 90�� ȸ��
                    rotatedDoor = new Vector2(door.y, Size.x - door.x);
                    break;
                case 2: // 180�� ȸ��
                    rotatedDoor = new Vector2(Size.x - door.x, Size.y - door.y);
                    break;
                case 3: // 270�� ȸ��
                    rotatedDoor = new Vector2(Size.y - door.y, door.x);
                    break;
                default: // 0�� ȸ��
                    rotatedDoor = door;
                    break;
            }
            rotatedDoors.Add(rotatedDoor);
        }
        return rotatedDoors;
    }
}
