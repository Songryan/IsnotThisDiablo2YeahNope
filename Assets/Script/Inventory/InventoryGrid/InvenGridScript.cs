using UnityEngine; // Unity ���� ����� ���

public class InvenGridScript : MonoBehaviour // InvenGridScript Ŭ���� ����, MonoBehaviour�� ���
{
    /* �� �� ���
     * �κ��丮 �׸��� ���� (�Ϸ�)
     * �г� �߰� (�Ϸ�)
     * ���� �κ��丮 ��� (�Ϸ�)
     * 
     * �׽�Ʈ ������ ���� (�Ϸ�)
     * ������ �̵� (�Ϸ�)
     * ������ ��� (�Ϸ�)
     * ������ �������� (�Ϸ�)
     * ������ ��ȯ (�Ϸ�)
     * �巡�� Ȯ�� �� ���̶���Ʈ ���� (�Ϸ�)
     * ���� ���̶���Ʈ �ٽ� ���� *�ʹ� ��� �б� �����* (��ü�� �Ϸ�) *****
     * 
     * �����ۿ� ��ũ�� ����Ʈ UI ����� (�Ϸ�)
     * ������ ��ư (�Ϸ�)
     * ��ư���� ������ ��� ���� (�Ϸ�)
     * �׸��忡 �������� ���� �� ����Ʈ���� ��ư ���� (�Ϸ�)
     * ��ư ������Ʈ Ǯ | ��ư �� ������ ��� (�Ϸ�)
     * ����Ʈ�� ������ �ٽ� ��� (�Ϸ�)
     * ������ ���� �г� �߰� (�Ϸ�)
     * 
     * ������ ��� �߰� **���߿�
     * ������ ��� �������� �߰� (�Ϸ� **�ٵ�� �ʿ�)
     * 
     * ��ü ������ Ŭ���� ��� "����, ����" �����
     * �� ���� ��踦 �߰��� �� StatPanel ���� ũ�� ����
     * �� ���� ������ ����, �̸� �� ������ �߰� (�Ϸ�)
     * 
     * ������ ��谡 �÷��̾� ��迡 ������ ��ġ���� �ϱ� *���߿�*
     * ������ ������ ����� (�Ϸ�)
     * ���� ������ ������ ����� (��ü�� �Ϸ�) **������ Ŭ���� �� ��� Ȯ�� �� �߰� �۾� �ʿ�
     * ���õ� �������� ���� �� �׸����� �������� ������� ������ �ϱ� (�Ϸ�)
     * ����Ʈ�� ������ ������ ����� �߰��ϴ� ��ư �����
     * 
     * ���� ����Ʈ �߰� (�Ϸ�)
     * ���İ� �����Ͽ� ����Ʈ�� ������ �߰� �ٽ� �۾� (�Ϸ�)
     */

    /* ���� ����
     * ǰ���� ���� ���� ���� ��� �ؽ�Ʈ ���� ***���߿� ���� ������ �Լ��� �����
     * Ư���� ����� ������ ���� *�ſ� �����, ��ü �ý��� ���ۼ� �ʿ�*
     * �׷��� �߰�
     * ������ ȸ��
     * ��ǰ�� ������ ���� �� ��� �˾� �߰�
     * ����/�ε� ��� �߰� *�����/���� ����*
     * IntVector2 �޼��� �� �Ű����� ���� *���� ��*
     * �׸��� ���� �߰� *�����*
     */

    public GameObject[,] slotGrid; // ���� �׸��带 ������ 2���� �迭
    public GameObject slotPrefab; // ���� �������� ������ ����
    public IntVector2 gridSize; // �׸��� ũ�⸦ ������ ����
    public float slotSize; // ���� ũ�⸦ ������ ����
    public float edgePadding; // �����ڸ� ������ ������ ����

    public void Awake() // Unity���� ��ũ��Ʈ�� ��� �� ȣ��Ǵ� �޼���
    {
        slotGrid = new GameObject[gridSize.x, gridSize.y]; // �׸��� ũ�⿡ ���� ���� �׸��� �迭 �ʱ�ȭ
        ResizePanel(); // �г� ũ�� ����
        CreateSlots(); // ���� ����
        GetComponent<InvenGridManager>().gridSize = gridSize; // �κ��丮 �׸��� �Ŵ����� �׸��� ũ�� ����
    }

    private void CreateSlots() // ������ �����ϴ� �޼���
    {
        for (int y = 0; y < gridSize.y; y++) // �׸����� y���� ��ȸ�ϸ�
        {
            for (int x = 0; x < gridSize.x; x++) // �׸����� x���� ��ȸ�ϸ�
            {
                GameObject obj = (GameObject)Instantiate(slotPrefab); // ���� �������� �ν��Ͻ�ȭ

                obj.transform.name = "slot[" + x + "," + y + "]"; // ���� �̸� ����
                obj.transform.SetParent(this.transform); // ������ �θ� ���� Ʈ���������� ����
                RectTransform rect = obj.transform.GetComponent<RectTransform>(); // ������ RectTransform ������Ʈ�� ������
                rect.localPosition = new Vector3(x * slotSize + edgePadding, y * slotSize + edgePadding, 0); // ������ ��ġ ����
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize); // ������ ���� ũ�� ����
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize); // ������ ���� ũ�� ����
                obj.GetComponent<RectTransform>().localScale = Vector3.one; // ������ ������ ����
                obj.GetComponent<SlotScript>().gridPos = new IntVector2(x, y); // ������ �׸��� ��ġ ����
                slotGrid[x, y] = obj; // ������ ���� �׸��� �迭�� �߰�
            }
        }
        GetComponent<InvenGridManager>().slotGrid = slotGrid; // �κ��丮 �׸��� �Ŵ����� ���� �׸��� ����
    }

    private void ResizePanel() // �г� ũ�⸦ �����ϴ� �޼���
    {
        float width, height; // �г��� �ʺ�� ���̸� ������ ����
        width = (gridSize.x * slotSize) + (edgePadding * 2); // �׸��� ũ�⿡ ���� �ʺ� ���
        height = (gridSize.y * slotSize) + (edgePadding * 2); // �׸��� ũ�⿡ ���� ���� ���

        RectTransform rect = GetComponent<RectTransform>(); // ���� Ʈ�������� RectTransform ������Ʈ�� ������
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width); // �г��� ���� ũ�� ����
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height); // �г��� ���� ũ�� ����
        //rect.localScale = Vector3.one; // �г��� ������ ����
        rect.localScale = new Vector3(0.32f, 0.32f, 0.32f); // �г��� ������ ����
    }
}
