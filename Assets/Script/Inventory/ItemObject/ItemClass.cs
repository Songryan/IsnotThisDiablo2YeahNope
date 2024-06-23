using UnityEngine; // Unity ���� ����� ���

[System.Serializable] // �� Ŭ������ ����ȭ �����ϵ��� ����
public class ItemClass
{
    public int GlobalID; // �۷ι� ������ ID
    [HideInInspector] public int CategoryID; // ī�װ� ID, �ν����Ϳ��� ����
    [HideInInspector] public string CategoryName; // ī�װ� �̸�, �ν����Ϳ��� ����
    [HideInInspector] public int TypeID; // Ÿ�� ID, �ν����Ϳ��� ����
    public string TypeName; // Ÿ�� �̸�
    public int Level; // ������ ����
    [Range(0, 3)] public int qualityInt; // ǰ�� ��, 0���� 3 ������ ��
    [HideInInspector] public IntVector2 Size; // ������ ũ��, �ν����Ϳ��� ����
    [HideInInspector] public Sprite Icon; // ������ ������, �ν����Ϳ��� ����
    public string SerialID; // ������ �ø��� ID

    // �⺻ ������, �����ڸ� ��������� �������� ������ ���� �߻� (������ �Ҹ�)
    public ItemClass() { }

    // �������� �����Ͽ� �� �������� �����ϴ� ������
    public ItemClass(ItemClass passedItem)
    {
        GlobalID = passedItem.GlobalID; // �۷ι� ID ����
        Level = passedItem.Level; // ���� ����
        qualityInt = passedItem.qualityInt; // ǰ�� ����
    }

    // ǰ�� ���ڿ��� ��ȯ�ϴ� �Ӽ�
    public string qualityStr
    {
        get
        {
            switch (qualityInt)
            {
                case 0: return "Broken";
                case 1: return "Normal";
                case 2: return "Magic";
                case 3: return "Rare";
                default: return null;
            }
        }
    }

    // ������ ���� �����ϴ� ���� �޼���
    public static void SetItemValues(ItemClass item, int ID, int lvl, int quality)
    {
        item.GlobalID = ID; // �۷ι� ID ����
        item.Level = lvl; // ���� ����
        item.qualityInt = quality; // ǰ�� ����
        GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>().PassItemData(ref item); // ������ �����ͺ��̽����� ������ ������ ��������
    }

    // ������ ���� �����ϴ� �� �ٸ� ���� �޼��� (ID, ����, ǰ�� ����)
    public static void SetItemValues(ItemClass item)
    {
        GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>().PassItemData(ref item); // ������ �����ͺ��̽����� ������ ������ ��������
    }
}
