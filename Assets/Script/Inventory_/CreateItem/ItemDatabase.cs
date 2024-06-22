using UnityEngine; // Unity ���� ����� ���
using System.Collections.Generic; // ���׸� �÷����� ���
using System; // �⺻ �ý��� ���̺귯���� ���

public class ItemDatabase : MonoBehaviour // ItemDatabase Ŭ���� ����, MonoBehaviour�� ���
{
    public TextAsset file; // CSV ������ ������ ����
    public List<int> GlobalIDList; // �۷ι� ID ����Ʈ
    public List<string> TypeNameList; // Ÿ�� �̸� ����Ʈ
    public List<IntVector2> SizeList; // ũ�� ����Ʈ
    public List<Sprite> IconList; // ������ ����Ʈ

    private void Awake() // Unity���� ��ũ��Ʈ�� ��� �� ȣ��Ǵ� �޼���
    {
        Load(file); // CSV ������ �ε�
    }

    public class Row // CSV ������ �� ���� ��Ÿ���� Ŭ����
    {
        public int GlobalID; // �۷ι� ID
        public int CategoryID; // ī�װ� ID
        public string CategoryName; // ī�װ� �̸�
        public int TypeID; // Ÿ�� ID
        public string TypeName; // Ÿ�� �̸�
        public IntVector2 Size; // ũ��
        public Sprite Icon; // ������
        public string SerailID; // �ø��� ID
    }

    public List<Row> rowList = new List<Row>(); // CSV ������ ��� ���� ������ ����Ʈ
    bool isLoaded = false; // ������ �ε�Ǿ����� ���θ� ������ ����

    public bool IsLoaded() // ������ �ε�Ǿ����� Ȯ���ϴ� �޼���
    {
        return isLoaded;
    }

    public List<Row> GetRowList() // �� ����Ʈ�� ��ȯ�ϴ� �޼���
    {
        return rowList;
    }

    public void Load(TextAsset csv) // CSV ������ �ε��ϴ� �޼���
    {
        rowList.Clear(); // ���� ����Ʈ�� �ʱ�ȭ
        string[][] grid = CsvParser2.Parse(csv.text); // CSV ������ �Ľ��Ͽ� 2���� �迭�� ��ȯ
        for (int i = 1; i < grid.Length; i++) // ù ��° ���� �����ϰ� �� ���� ��ȸ
        {
            Row row = new Row(); // ���ο� �� ��ü ����
            row.GlobalID = Int32.Parse(grid[i][0]); // �۷ι� ID ����
            GlobalIDList.Add(row.GlobalID); // �۷ι� ID ����Ʈ�� �߰�
            row.CategoryID = Int32.Parse(grid[i][1]); // ī�װ� ID ����
            row.CategoryName = grid[i][2]; // ī�װ� �̸� ����
            row.TypeID = Int32.Parse(grid[i][3]); // Ÿ�� ID ����
            row.TypeName = grid[i][4]; // Ÿ�� �̸� ����
            TypeNameList.Add(row.TypeName); // Ÿ�� �̸� ����Ʈ�� �߰�
            row.Size = new IntVector2(Int32.Parse(grid[i][5]), Int32.Parse(grid[i][6])); // ũ�� ����
            SizeList.Add(row.Size); // ũ�� ����Ʈ�� �߰�
            row.Icon = Resources.Load<Sprite>("ItemIcons/" + grid[i][4]); // ������ �ε�
            IconList.Add(row.Icon); // ������ ����Ʈ�� �߰�
            row.SerailID = grid[i][7]; // �ø��� ID ����
            rowList.Add(row); // �� ����Ʈ�� �߰�
        }
        isLoaded = true; // ���� �ε� �Ϸ�
    }

    public void PassItemData(ref ItemClass item) // ������ �����͸� �����ϴ� �޼���
    {
        int ID = item.GlobalID; // �۷ι� ID�� ������
        item.CategoryID = rowList[ID].CategoryID; // ī�װ� ID ����
        item.CategoryName = rowList[ID].CategoryName; // ī�װ� �̸� ����
        item.TypeID = rowList[ID].TypeID; // Ÿ�� ID ����
        item.TypeName = rowList[ID].TypeName; // Ÿ�� �̸� ����
        item.Size = rowList[ID].Size; // ũ�� ����
        item.Icon = rowList[ID].Icon; // ������ ����
        item.SerialID = rowList[ID].SerailID; // �ø��� ID ����
    }

    public int NumRows() // ���� ���� ��ȯ�ϴ� �޼���
    {
        return rowList.Count;
    }

    public Row GetAt(int i) // Ư�� �ε����� ���� ��ȯ�ϴ� �޼���
    {
        if (rowList.Count <= i)
            return null; // �ε����� ������ ����� null ��ȯ
        return rowList[i]; // �ش� �ε����� �� ��ȯ
    }
}
