using UnityEngine; // Unity ���� ����� ���
using UnityEngine.UI; // UI ��Ҹ� ���

public class InvenGridManager : MonoBehaviour // InvenGridManager Ŭ���� ����, MonoBehaviour�� ���
{
    public GameObject[,] slotGrid; // ���� �׸��带 ������ 2���� �迭
    public GameObject highlightedSlot; // ������ ������ ������ ����
    public Transform dropParent; // ��� �θ� Ʈ�������� ������ ����
    [HideInInspector]
    public IntVector2 gridSize; // �׸��� ũ�⸦ ������ ����, �ν����Ϳ��� ����

    public ItemListManager listManager; // ������ ����Ʈ �Ŵ����� ������ ����
    public GameObject selectedButton; // ���õ� ��ư�� ������ ����

    private IntVector2 totalOffset, checkSize, checkStartPos; // ���� üũ �� �������� ������ ����
    private IntVector2 otherItemPos, otherItemSize; // �ٸ� �������� ��ġ �� ũ�⸦ ������ ����

    private int checkState; // üũ ���¸� ������ ����
    private bool isOverEdge = false; // �׸����� �����ڸ��� �Ѿ����� ���θ� ������ ����

    public ItemOverlayScript overlayScript; // ������ �������� ��ũ��Ʈ�� ������ ����

    /* �� �� ���
     * �������� ��ȯ�� �� ColorChangeLoop�� �ٸ� �������� �Ű������� �����ϵ��� ���� *1
     * CheckArea()�� SlotCheck()�� RefreshColor() ���η� �̵� *2
     * *3�� CheckArea()�� ���� ������ ����. SwapItem()���� ������ ����ϹǷ� ���ۼ� �ʿ�.
     */
    private void Start() // Unity���� ��ũ��Ʈ�� ó�� ����� �� ȣ��Ǵ� �޼���
    {
        ItemButtonScript.invenManager = this; // ItemButtonScript�� invenManager�� ���� �ν��Ͻ��� ����
    }

    private void Update() // Unity���� �� �����Ӹ��� ȣ��Ǵ� �޼���
    {
        //if (Input.GetMouseButtonUp(0)) // ���� ���콺 ��ư�� ���� ��
        if (Input.GetMouseButtonDown(0)) // ���� ���콺 ��ư�� ���� ��
        {
            if (highlightedSlot != null && ItemScript.selectedItem != null && !isOverEdge) // ������ ���԰� ���õ� �������� �ְ� �����ڸ��� ���� ���� ���
            {
                switch (checkState)
                {
                    case 0: // �� ���Կ� ����
                        StoreItem(ItemScript.selectedItem); // ������ ����
                        ColorChangeLoop(SlotColorHighlights.Blue, ItemScript.selectedItemSize, totalOffset); // ���� ���� ����
                        ItemScript.ResetSelectedItem(); // ���õ� ������ �ʱ�ȭ
                        RemoveSelectedButton(); // ���õ� ��ư ����
                        break;
                    case 1: // ������ ��ȯ
                        ItemScript.SetSelectedItem(SwapItem(ItemScript.selectedItem)); // ������ ��ȯ
                        SlotSectorScript.sectorScript.PosOffset(); // ��ġ ������ ����
                        ColorChangeLoop(SlotColorHighlights.Gray, otherItemSize, otherItemPos); // ���� ���� ����
                        RefrechColor(true); // ���� ����
                        RemoveSelectedButton(); // ���õ� ��ư ����
                        break;
                }
            }
            // �������� ��������
            else if (highlightedSlot != null && ItemScript.selectedItem == null && highlightedSlot.GetComponent<SlotScript>().isOccupied == true)
            {
                ColorChangeLoop(SlotColorHighlights.Gray, highlightedSlot.GetComponent<SlotScript>().storedItemSize, highlightedSlot.GetComponent<SlotScript>().storedItemStartPos); // ���� ���� ����
                ItemScript.SetSelectedItem(GetItem(highlightedSlot)); // ������ ��������
                SlotSectorScript.sectorScript.PosOffset(); // ��ġ ������ ����
                RefrechColor(true); // ���� ����
            }
        }
    }

    private void CheckArea(IntVector2 itemSize) // ������ ũ�⸦ üũ�ϴ� �޼���
    {
        IntVector2 halfOffset;
        IntVector2 overCheck;
        halfOffset.x = (itemSize.x - (itemSize.x % 2 == 0 ? 0 : 1)) / 2;
        halfOffset.y = (itemSize.y - (itemSize.y % 2 == 0 ? 0 : 1)) / 2;
        totalOffset = highlightedSlot.GetComponent<SlotScript>().gridPos - (halfOffset + SlotSectorScript.posOffset);
        checkStartPos = totalOffset;
        checkSize = itemSize;
        overCheck = totalOffset + itemSize;
        isOverEdge = false;
        // ������ �������� �׸��� �ۿ� �ִ��� üũ
        if (overCheck.x > gridSize.x)
        {
            checkSize.x = gridSize.x - totalOffset.x;
            isOverEdge = true;
        }
        if (totalOffset.x < 0)
        {
            checkSize.x = itemSize.x + totalOffset.x;
            checkStartPos.x = 0;
            isOverEdge = true;
        }
        if (overCheck.y > gridSize.y)
        {
            checkSize.y = gridSize.y - totalOffset.y;
            isOverEdge = true;
        }
        if (totalOffset.y < 0)
        {
            checkSize.y = itemSize.y + totalOffset.y;
            checkStartPos.y = 0;
            isOverEdge = true;
        }
    }

    private int SlotCheck(IntVector2 itemSize) // ������ üũ�ϴ� �޼���
    {
        GameObject obj = null;
        SlotScript instanceScript;
        if (!isOverEdge)
        {
            for (int y = 0; y < itemSize.y; y++)
            {
                for (int x = 0; x < itemSize.x; x++)
                {
                    instanceScript = slotGrid[checkStartPos.x + x, checkStartPos.y + y].GetComponent<SlotScript>();
                    if (instanceScript.isOccupied)
                    {
                        if (obj == null)
                        {
                            obj = instanceScript.storedItemObject;
                            otherItemPos = instanceScript.storedItemStartPos;
                            otherItemSize = obj.GetComponent<ItemScript>().item.Size;
                        }
                        else if (obj != instanceScript.storedItemObject)
                            return 2; // üũ ������ 1�� �̻��� �������� �ִ� ���
                    }
                }
            }
            if (obj == null)
                return 0; // üũ ������ ����ִ� ���
            else
                return 1; // üũ ������ 1���� �������� �ִ� ���
        }
        return 2; // üũ ������ �׸��带 ���� ���
    }

    public void RefrechColor(bool enter) // ������ �����ϴ� �޼���
    {
        if (enter)
        {
            CheckArea(ItemScript.selectedItemSize); // ������ ũ�� üũ
            checkState = SlotCheck(checkSize); // ���� üũ ���� ����
            switch (checkState)
            {
                case 0: ColorChangeLoop(SlotColorHighlights.Green, checkSize, checkStartPos); break; // ������ �������� ���� ���
                case 1:
                    ColorChangeLoop(SlotColorHighlights.Yellow, otherItemSize, otherItemPos); // ������ 1���� �������� �ְ� ��ȯ ������ ���
                    ColorChangeLoop(SlotColorHighlights.Green, checkSize, checkStartPos);
                    break;
                case 2: ColorChangeLoop(SlotColorHighlights.Red, checkSize, checkStartPos); break; // ��ġ�� ��ȿ���� ���� ��� (2�� �̻��� �������� �ְų� �׸��带 ���� ���)
            }
        }
        else // �����Ͱ� ������ ���� �� ���� �ʱ�ȭ
        {
            isOverEdge = false;
            //checkArea(); // ���� ����� ���� �ּ� ó����. ���Ե��� ������ ���� �߻� ����

            ColorChangeLoop2(checkSize, checkStartPos);
            if (checkState == 1)
            {
                ColorChangeLoop(SlotColorHighlights.Blue2, otherItemSize, otherItemPos);
            }
        }
    }

    public void ColorChangeLoop(Color32 color, IntVector2 size, IntVector2 startPos) // ���� ���� ����
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                slotGrid[startPos.x + x, startPos.y + y].GetComponent<Image>().color = color;
            }
        }
    }

    public void ColorChangeLoop2(IntVector2 size, IntVector2 startPos) // ���� ���� ���� 2
    {
        GameObject slot;
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                slot = slotGrid[startPos.x + x, startPos.y + y];
                if (slot.GetComponent<SlotScript>().isOccupied != false)
                {
                    slotGrid[startPos.x + x, startPos.y + y].GetComponent<Image>().color = SlotColorHighlights.Blue2;
                }
                else
                {
                    slotGrid[startPos.x + x, startPos.y + y].GetComponent<Image>().color = SlotColorHighlights.Gray;
                }
            }
        }
    }

    private void StoreItem(GameObject item) // �������� �����ϴ� �޼���
    {
        SlotScript instanceScript;
        IntVector2 itemSizeL = item.GetComponent<ItemScript>().item.Size;
        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
            {
                // �� ������ �Ű����� ����
                instanceScript = slotGrid[totalOffset.x + x, totalOffset.y + y].GetComponent<SlotScript>();
                instanceScript.storedItemObject = item;
                instanceScript.storedItemClass = item.GetComponent<ItemScript>().item;
                instanceScript.storedItemSize = itemSizeL;
                instanceScript.storedItemStartPos = totalOffset;
                instanceScript.isOccupied = true;
                slotGrid[totalOffset.x + x, totalOffset.y + y].GetComponent<Image>().color = SlotColorHighlights.Gray;
            }
        }
        // ��ӵ� ������ �Ű����� ����
        item.transform.SetParent(dropParent);
        item.GetComponent<RectTransform>().pivot = Vector2.zero;
        item.transform.position = slotGrid[totalOffset.x, totalOffset.y].transform.position;
        item.GetComponent<CanvasGroup>().alpha = 1f;
        overlayScript.UpdateOverlay(highlightedSlot.GetComponent<SlotScript>().storedItemClass);
    }

    private GameObject GetItem(GameObject slotObject) // �������� �������� �޼���
    {
        SlotScript slotObjectScript = slotObject.GetComponent<SlotScript>();
        GameObject retItem = slotObjectScript.storedItemObject;
        IntVector2 tempItemPos = slotObjectScript.storedItemStartPos;
        IntVector2 itemSizeL = retItem.GetComponent<ItemScript>().item.Size;

        SlotScript instanceScript;
        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
            {
                // �� ������ �Ű����� �ʱ�ȭ
                instanceScript = slotGrid[tempItemPos.x + x, tempItemPos.y + y].GetComponent<SlotScript>();
                instanceScript.storedItemObject = null;
                instanceScript.storedItemSize = IntVector2.Zero;
                instanceScript.storedItemStartPos = IntVector2.Zero;
                instanceScript.storedItemClass = null;
                instanceScript.isOccupied = false;
            }
        }
        // ��ȯ�� ������ �Ű����� ����
        retItem.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        retItem.GetComponent<CanvasGroup>().alpha = 0.5f;
        retItem.transform.position = Input.mousePosition;
        overlayScript.UpdateOverlay(null);
        return retItem;
    }

    private GameObject SwapItem(GameObject item) // �������� ��ȯ�ϴ� �޼���
    {
        GameObject tempItem;
        tempItem = GetItem(slotGrid[otherItemPos.x, otherItemPos.y]);
        StoreItem(item);
        return tempItem;
    }

    public void RemoveSelectedButton() // ���õ� ��ư�� �����ϴ� �޼���
    {
        if (selectedButton != null)
        {
            listManager.RevomeItemFromList(selectedButton.GetComponent<ItemButtonScript>().item); // ����Ʈ���� ������ ����
            listManager.RemoveButton(selectedButton); // ��ư ����
            listManager.sortManager.SortAndFilterList(); // ����Ʈ ���� �� ���͸�
            selectedButton = null; // ���õ� ��ư �ʱ�ȭ
        }
    }
}

public struct SlotColorHighlights // ���� ���� ���̶���Ʈ�� �����ϴ� ����ü
{
    public static Color32 Green
    { get { return new Color32(127, 223, 127, 255); } }
    public static Color32 Yellow
    { get { return new Color32(223, 223, 63, 255); } }
    public static Color32 Red
    { get { return new Color32(223, 127, 127, 255); } }
    public static Color32 Blue
    { get { return new Color32(159, 159, 223, 255); } }
    public static Color32 Blue2
    { get { return new Color32(0, 0, 200, 60); } }
    public static Color32 Gray
    { get { return new Color32(0, 0, 200, 0); } }
}
