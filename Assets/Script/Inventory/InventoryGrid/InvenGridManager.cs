using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine; // Unity 엔진 기능을 사용
using UnityEngine.UI; // UI 요소를 사용

public class InvenGridManager : MonoBehaviour // InvenGridManager 클래스 정의, MonoBehaviour를 상속
{
    public GameObject[,] slotGrid; // 슬롯 그리드를 저장할 2차원 배열
    public GameObject highlightedSlot; // 강조된 슬롯을 저장할 변수
    public Transform dropParent; // 드롭 부모 트랜스폼을 저장할 변수
    public Transform dropParent_Hover; // 드롭 부모 트랜스폼을 저장할 변수
    [HideInInspector]
    public IntVector2 gridSize; // 그리드 크기를 저장할 변수, 인스펙터에서 숨김

    public ItemListManager listManager; // 아이템 리스트 매니저를 저장할 변수
    public GameObject selectedButton; // 선택된 버튼을 저장할 변수

    public IntVector2 totalOffset, checkSize, checkStartPos; // 슬롯 체크 및 오프셋을 저장할 변수
    public IntVector2 otherItemPos, otherItemSize; // 다른 아이템의 위치 및 크기를 저장할 변수

    private int checkState; // 체크 상태를 저장할 변수
    private bool isOverEdge = false; // 그리드의 가장자리를 넘었는지 여부를 저장할 변수

    public ItemOverlayScript overlayScript; // 아이템 오버레이 스크립트를 저장할 변수

    public bool checkCanEquip = true;
    public string pSlotType;

    public GameObject placeholder;
    public GameObject highlighter;

    /* 할 일 목록
     * 아이템을 교환할 때 ColorChangeLoop에 다른 아이템의 매개변수를 전달하도록 수정 *1
     * CheckArea()와 SlotCheck()를 RefreshColor() 내부로 이동 *2
     * *3을 CheckArea()의 로컬 변수로 변경. SwapItem()에서 변수를 사용하므로 재작성 필요.
     */
    private void Start() // Unity에서 스크립트가 처음 실행될 때 호출되는 메서드
    {
        pSlotType = transform.GetComponent<InvenGridScript>().slotType;
        ItemButtonScript.invenManager = this; // ItemButtonScript의 invenManager를 현재 인스턴스로 설정
    }

    private void Update() // Unity에서 매 프레임마다 호출되는 메서드
    {
        CheckEquipType();

        if (pSlotType.Equals("All"))
            checkCanEquip = true;

        //if (Input.GetMouseButtonUp(0)) // 왼쪽 마우스 버튼을 뗐을 때
        if (Input.GetMouseButtonDown(0) && checkCanEquip) // 왼쪽 마우스 버튼을 눌렀을 때 (모바일 환경을위해 변경)
        {
            // 누르자마자 오버레이 업데이트 (모바일 환경을위해 변경)
            if(highlightedSlot != null)
                overlayScript.UpdateOverlay(highlightedSlot.GetComponent<SlotScript>().storedItemClass);

            if (highlightedSlot != null && ItemScript.selectedItem != null && !isOverEdge) // 강조된 슬롯과 선택된 아이템이 있고 가장자리를 넘지 않은 경우
            {
                switch (checkState)
                {
                    case 0: // 빈 슬롯에 저장
                        // 인벤에 놓아 저장하는 순간 Json 다시 저장.
                        JsonDataManager.Instance.DeleteAndModifyJsonData(ItemScript.selectedItem.transform.GetComponent<ItemScript>().item.UniqueKey);
                        StoreItem(ItemScript.selectedItem); // 아이템 저장
                        ColorChangeLoop(SlotColorHighlights.Blue, ItemScript.selectedItemSize, totalOffset); // 슬롯 색상 변경
                        ItemScript.ResetSelectedItem(); // 선택된 아이템 초기화
                        RemoveSelectedButton(); // 선택된 버튼 제거
                        ColorReChanger(); // 색상 재갱신
                        break;
                    case 1: // 아이템 교환
                        ItemScript.SetSelectedItem(SwapItem(ItemScript.selectedItem)); // 아이템 교환
                        SlotSectorScript.sectorScript.PosOffset(); // 위치 오프셋 설정
                        ColorChangeLoop(SlotColorHighlights.Gray, otherItemSize, otherItemPos); // 슬롯 색상 변경
                        RefrechColor(true); // 색상 갱신
                        RemoveSelectedButton(); // 선택된 버튼 제거
                        ColorReChanger(); // 색상 재갱신
                        break;
                }
            }
            // 아이템을 가져오기
            else if (highlightedSlot != null && ItemScript.selectedItem == null && highlightedSlot.GetComponent<SlotScript>().isOccupied == true)
            {
                ColorChangeLoop(SlotColorHighlights.Gray, highlightedSlot.GetComponent<SlotScript>().storedItemSize, highlightedSlot.GetComponent<SlotScript>().storedItemStartPos); // 슬롯 색상 변경
                ItemScript.SetSelectedItem(GetItem(highlightedSlot)); // 아이템 가져오기
                SlotSectorScript.sectorScript.PosOffset(); // 위치 오프셋 설정
                RefrechColor(true); // 색상 갱신
                ColorReChanger(); // 색상 재갱신
            }
        }
    }

    public void InvenDataPostioning(int x, int y)
    {
        ToInvenDataStoreItem(ItemScript.selectedItem, x, y); // 아이템 저장
        ColorChangeLoop(SlotColorHighlights.Blue2, ItemScript.selectedItemSize, totalOffset); // 슬롯 색상 변경
        ItemScript.ResetSelectedItem(); // 선택된 아이템 초기화
        RemoveSelectedButton(); // 선택된 버튼 제거
        ColorReChanger(); // 색상 재갱신
    }

    public void ColorReChanger()
    {
        foreach (Transform child in transform)
        {
            child.transform.GetComponent<SlotScript>().EquipInvenColorChanger();
        }
    }

    private void CheckArea(IntVector2 itemSize) // 아이템 크기를 체크하는 메서드
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
        // 저장할 아이템이 그리드 밖에 있는지 체크
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

    private int SlotCheck(IntVector2 itemSize) // 슬롯을 체크하는 메서드
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
                            return 2; // 체크 영역에 1개 이상의 아이템이 있는 경우
                    }
                }
            }
            if (obj == null)
                return 0; // 체크 영역이 비어있는 경우
            else
                return 1; // 체크 영역에 1개의 아이템이 있는 경우
        }
        return 2; // 체크 영역이 그리드를 넘은 경우
    }

    public void RefrechColor(bool enter) // 색상을 갱신하는 메서드
    {
        if (enter)
        {
            CheckArea(ItemScript.selectedItemSize); // 아이템 크기 체크
            checkState = SlotCheck(checkSize); // 슬롯 체크 상태 설정
            switch (checkState)
            {
                case 0: ColorChangeLoop(SlotColorHighlights.Green, checkSize, checkStartPos); break; // 영역에 아이템이 없는 경우
                case 1:
                    ColorChangeLoop(SlotColorHighlights.Yellow, otherItemSize, otherItemPos); // 영역에 1개의 아이템이 있고 교환 가능한 경우
                    ColorChangeLoop(SlotColorHighlights.Green, checkSize, checkStartPos);
                    break;
                case 2: ColorChangeLoop(SlotColorHighlights.Red, checkSize, checkStartPos); break; // 위치가 유효하지 않은 경우 (2개 이상의 아이템이 있거나 그리드를 넘은 경우)
            }
        }
        else // 포인터가 슬롯을 떠날 때 색상 초기화
        {
            isOverEdge = false;
            //checkArea(); // 성능 향상을 위해 주석 처리됨. 포함되지 않으면 버그 발생 가능

            ColorChangeLoop2(checkSize, checkStartPos);
            if (checkState == 1)
            {
                ColorChangeLoop(SlotColorHighlights.Blue2, otherItemSize, otherItemPos);
            }
        }
    }

    public void RefrechColor_Equip(bool enter) // 색상을 갱신하는 메서드
    {
        if (enter)
        {
            CheckArea(ItemScript.selectedItemSize); // 아이템 크기 체크
            checkState = SlotCheck(checkSize); // 슬롯 체크 상태 설정
            //switch (checkState)
            //{
            //    case 0: ColorChangeLoop(SlotColorHighlights.Green, checkSize, checkStartPos); break; // 영역에 아이템이 없는 경우
            //    case 1:
            //        ColorChangeLoop(SlotColorHighlights.Yellow, otherItemSize, otherItemPos); // 영역에 1개의 아이템이 있고 교환 가능한 경우
            //        ColorChangeLoop(SlotColorHighlights.Green, checkSize, checkStartPos);
            //        break;
            //    case 2: ColorChangeLoop(SlotColorHighlights.Red, checkSize, checkStartPos); break; // 위치가 유효하지 않은 경우 (2개 이상의 아이템이 있거나 그리드를 넘은 경우)
            //}
        }
        else // 포인터가 슬롯을 떠날 때 색상 초기화
        {
            isOverEdge = false;
            //checkArea(); // 성능 향상을 위해 주석 처리됨. 포함되지 않으면 버그 발생 가능

            //ColorChangeLoop2(checkSize, checkStartPos);
            //if (checkState == 1)
            //{
            //    ColorChangeLoop(SlotColorHighlights.Blue2, otherItemSize, otherItemPos);
            //}
        }
    }

    public void ColorChangeLoop(Color32 color, IntVector2 size, IntVector2 startPos) // 색상 변경 루프
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                slotGrid[startPos.x + x, startPos.y + y].GetComponent<Image>().color = color;
            }
        }
    }

    public void ColorChangeLoop2(IntVector2 size, IntVector2 startPos) // 색상 변경 루프 2
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

    private void StoreItem(GameObject item) // 아이템을 저장하는 메서드
    {
        SlotScript instanceScript;
        IntVector2 itemSizeL = item.GetComponent<ItemScript>().item.Size;
        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
            {
                // 각 슬롯의 매개변수 설정
                instanceScript = slotGrid[totalOffset.x + x, totalOffset.y + y].GetComponent<SlotScript>();
                instanceScript.storedItemObject = item;
                instanceScript.storedItemClass = item.GetComponent<ItemScript>().item;
                instanceScript.storedItemSize = itemSizeL;
                instanceScript.storedItemStartPos = totalOffset;
                instanceScript.isOccupied = true;
                slotGrid[totalOffset.x + x, totalOffset.y + y].GetComponent<Image>().color = SlotColorHighlights.Gray;
            }
        }
        // 드롭된 아이템 매개변수 설정
        item.transform.SetParent(dropParent);
        item.GetComponent<RectTransform>().pivot = Vector2.zero;
        item.transform.position = slotGrid[totalOffset.x, totalOffset.y].transform.position;
        item.GetComponent<CanvasGroup>().alpha = 1f;
        overlayScript.UpdateOverlay(highlightedSlot.GetComponent<SlotScript>().storedItemClass);
    }

    private void ToInvenDataStoreItem(GameObject item, int offsetX, int offsetY) // 아이템을 저장하는 메서드
    {
        SlotScript instanceScript;
        IntVector2 itemSizeL = item.GetComponent<ItemScript>().item.Size;
        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
            {
                // 각 슬롯의 매개변수 설정
                instanceScript = slotGrid[offsetX + x, offsetY + y].GetComponent<SlotScript>();
                instanceScript.storedItemObject = item;
                instanceScript.storedItemClass = item.GetComponent<ItemScript>().item;
                instanceScript.storedItemSize = itemSizeL;
                instanceScript.storedItemStartPos = new IntVector2(offsetX, offsetY);
                instanceScript.isOccupied = true;
                slotGrid[offsetX + x, offsetY + y].GetComponent<Image>().color = SlotColorHighlights.Gray;
            }
        }
        // 드롭된 아이템 매개변수 설정
        item.transform.SetParent(dropParent);
        item.GetComponent<RectTransform>().pivot = Vector2.zero;
        item.transform.position = slotGrid[offsetX, offsetY].transform.position;
        item.GetComponent<CanvasGroup>().alpha = 1f;
        //overlayScript.UpdateOverlay(highlightedSlot.GetComponent<SlotScript>().storedItemClass);
    }

    private GameObject GetItem(GameObject slotObject) // 아이템을 가져오는 메서드
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
                // 각 슬롯의 매개변수 초기화
                instanceScript = slotGrid[tempItemPos.x + x, tempItemPos.y + y].GetComponent<SlotScript>();
                instanceScript.storedItemObject = null;
                instanceScript.storedItemSize = IntVector2.Zero;
                instanceScript.storedItemStartPos = IntVector2.Zero;
                instanceScript.storedItemClass = null;
                instanceScript.isOccupied = false;
            }
        }
        // 반환된 아이템 매개변수 설정
        retItem.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        retItem.GetComponent<CanvasGroup>().alpha = 0.5f;
        retItem.transform.position = Input.mousePosition;
        //overlayScript.UpdateOverlay(null);
        return retItem;
    }

    private GameObject SwapItem(GameObject item) // 아이템을 교환하는 메서드
    {
        GameObject tempItem;
        tempItem = GetItem(slotGrid[otherItemPos.x, otherItemPos.y]);
        StoreItem(item);
        return tempItem;
    }

    public void RemoveSelectedButton() // 선택된 버튼을 제거하는 메서드
    {
        if (selectedButton != null)
        {
            listManager.RevomeItemFromList(selectedButton.GetComponent<ItemButtonScript>().item); // 리스트에서 아이템 제거
            listManager.RemoveButton(selectedButton); // 버튼 제거
            listManager.sortManager.SortAndFilterList(); // 리스트 정렬 및 필터링
            selectedButton = null; // 선택된 버튼 초기화
        }
    }

    public bool CheckEquipType()
    {
        // 자식 없으면 걍 ture로 리턴.
        if (dropParent_Hover.childCount == 0)
            return true;

        string itemType = string.Empty;
        
        foreach (Transform child in dropParent_Hover.transform)
        {
            itemType = child.transform.GetComponent<ItemScript>().item.EquipCategory;
            itemType = new string(itemType.Where(c => char.IsLetter(c)).ToArray());
        }

        checkCanEquip = stringToEnum(itemType) == stringToEnum(pSlotType);
        return stringToEnum(itemType) == stringToEnum(pSlotType);
    }

    public SlotSectorScript.SlotType stringToEnum(string str)
    {
        SlotSectorScript.SlotType childItemType;

        switch (str)
        {
            case "Weapon":
                childItemType = SlotSectorScript.SlotType.Weapon;
                break;
            case "Shield":
                childItemType = SlotSectorScript.SlotType.Shield;
                break;
            case "Armor":
                childItemType = SlotSectorScript.SlotType.Armor;
                break;
            case "Glove":
                childItemType = SlotSectorScript.SlotType.Glove;
                break;
            case "Boot":
                childItemType = SlotSectorScript.SlotType.Boot;
                break;
            case "Helmet":
                childItemType = SlotSectorScript.SlotType.Helmet;
                break;
            case "Amulet":
                childItemType = SlotSectorScript.SlotType.Amulet;
                break;
            case "Ring":
                childItemType = SlotSectorScript.SlotType.Ring;
                break;
            case "Belt":
                childItemType = SlotSectorScript.SlotType.Belt;
                break;
            case "All":
                childItemType = SlotSectorScript.SlotType.All;
                break;
            default:
                childItemType = SlotSectorScript.SlotType.All;
                break;
        }

        return childItemType;
    }
}

public struct SlotColorHighlights // 슬롯 색상 하이라이트를 정의하는 구조체
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
    { get { return new Color32(0, 0, 200, 50); } }
    public static Color32 Gray
    { get { return new Color32(0, 0, 200, 0); } }
}
