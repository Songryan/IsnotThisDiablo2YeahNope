using UnityEngine; // Unity ���� ����� ���
using UnityEngine.EventSystems; // �̺�Ʈ �ý����� ���
using UnityEngine.UI; // UI ��Ҹ� ���

public class ItemScript : MonoBehaviour, IPointerClickHandler // ItemScript Ŭ���� ����, MonoBehaviour�� IPointerClickHandler�� ���
{
    private GameObject invenPanel; // �κ��丮 �г��� ������ ����
    public static GameObject selectedItem; // ���õ� �������� ������ ���� ����
    public static IntVector2 selectedItemSize; // ���õ� �������� ũ�⸦ ������ ���� ����
    public static bool isDragging = false; // �������� �巡�� ������ ���θ� ������ ���� ����

    private float slotSize; // ���� ũ�⸦ ������ ����

    public ItemClass item; // ������ Ŭ���� �ν��Ͻ��� ������ ����

    private void Awake() // Unity���� ��ũ��Ʈ�� ��� �� ȣ��Ǵ� �޼���
    {
        slotSize = GameObject.FindGameObjectWithTag("InvenPanel").GetComponent<InvenGridScript>().slotSize; // "InvenPanel" �±׸� ���� ������Ʈ���� ���� ũ�⸦ ������
    }

    public void SetItemObject(ItemClass passedItem) // ������ ������Ʈ�� �����ϴ� �޼���
    {
        RectTransform rect = GetComponent<RectTransform>(); // ���� ������Ʈ�� RectTransform ������Ʈ�� ������
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, passedItem.Size.x * slotSize); // ���� ũ�� ����
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, passedItem.Size.y * slotSize); // ���� ũ�� ����
        item = passedItem; // ������ ����
        GetComponent<Image>().sprite = passedItem.Icon; // ������ ������ ����
    }

    // obsolete
    public void OnPointerClick(PointerEventData eventData) // ���콺 Ŭ�� �̺�Ʈ ó�� �޼���
    {
        SetSelectedItem(this.gameObject); // ���õ� ������ ����
        CanvasGroup canvas = GetComponent<CanvasGroup>(); // CanvasGroup ������Ʈ�� ������
        canvas.blocksRaycasts = false; // ����ĳ��Ʈ ���� ����
        canvas.alpha = 0.5f; // ���� ����
    }

    private void Update() // Unity���� �� �����Ӹ��� ȣ��Ǵ� �޼���
    {
        if (isDragging) // �������� �巡�� ���� ���
        {
            selectedItem.transform.position = Input.mousePosition; // ���õ� �������� ��ġ�� ���콺 ��ġ�� ����
        }
    }

    public static void SetSelectedItem(GameObject obj) // ���õ� �������� �����ϴ� ���� �޼���
    {
        selectedItem = obj; // ���õ� ������ ����
        selectedItemSize = obj.GetComponent<ItemScript>().item.Size; // ���õ� ������ ũ�� ����
        isDragging = true; // �巡�� ������ ����
        obj.transform.SetParent(GameObject.FindGameObjectWithTag("DragParent").transform); // �θ� "DragParent" �±׸� ���� ������Ʈ�� ����
        obj.GetComponent<RectTransform>().localScale = Vector3.one; // ũ�� ����
    }

    public static void ResetSelectedItem() // ���õ� �������� �ʱ�ȭ�ϴ� ���� �޼���
    {
        selectedItem = null; // ���õ� ������ �ʱ�ȭ
        selectedItemSize = IntVector2.Zero; // ���õ� ������ ũ�� �ʱ�ȭ
        isDragging = false; // �巡�� �� ���� �ʱ�ȭ
    }
}