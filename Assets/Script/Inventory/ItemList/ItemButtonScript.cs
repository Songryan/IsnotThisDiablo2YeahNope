using UnityEngine; // Unity 엔진 기능을 사용
using UnityEngine.UI; // UI 요소를 사용
using UnityEngine.EventSystems; // 이벤트 시스템을 사용

public class ItemButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler // ItemButtonScript 클래스 정의, MonoBehaviour와 인터페이스를 상속
{
    public Button buttonComponent; // 버튼 컴포넌트를 저장할 변수
    public Text nameText; // 아이템 이름 텍스트를 저장할 변수
    public Image iconImage; // 아이템 아이콘 이미지를 저장할 변수
    public Text LvlText; // 아이템 레벨 텍스트를 저장할 변수
    public Text QualityText; // 아이템 품질 텍스트를 저장할 변수
    public Image QualityColor; // 아이템 품질 색상을 저장할 변수

    public ItemClass item; // 아이템 클래스 인스턴스를 저장할 변수
    private ItemListManager listManager; // 아이템 리스트 매니저를 저장할 변수
    public ObjectPoolScript itemEquipPool; // 아이템 장비 오브젝트 풀을 저장할 변수

    public static InvenGridManager invenManager; // 인벤토리 그리드 매니저를 저장할 정적 변수
    public static ItemOverlayScript overlayScript; // 아이템 오버레이 스크립트를 저장할 정적 변수

    public void OnPointerDown(PointerEventData eventData) // 마우스 버튼을 눌렀을 때 호출되는 메서드
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 버튼을 눌렀을 경우
        {
            if (ItemScript.selectedItem == null)
            {
                SpawnStoredItem(); // 선택된 아이템이 없으면 저장된 아이템을 생성
            }
            listManager.AddSelectedItemToList();
            if (ItemScript.selectedItem != null && invenManager.selectedButton != this.gameObject) // 리스트에서 아이템이 선택된 경우 버튼 초기화
            {
                invenManager.selectedButton.GetComponent<CanvasGroup>().alpha = 1f;
                invenManager.selectedButton = null;
                listManager.itemEquipPool.ReturnObject(ItemScript.selectedItem);
                SpawnStoredItem();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) // 마우스가 버튼 위에 있을 때 호출되는 메서드
    {
        overlayScript.UpdateOverlay(item); // 오버레이를 아이템 정보로 업데이트
    }

    public void OnPointerExit(PointerEventData eventData) // 마우스가 버튼을 떠났을 때 호출되는 메서드
    {
        overlayScript.UpdateOverlay(null); // 오버레이를 초기화
    }

    private void SpawnStoredItem() // 저장된 아이템을 생성하는 메서드
    {
        GameObject newItem = itemEquipPool.GetObject(); // 오브젝트 풀에서 새 아이템을 가져옴
        newItem.GetComponent<ItemScript>().SetItemObject(item); // 아이템 객체 설정

        ItemScript.SetSelectedItem(newItem); // 선택된 아이템 설정
        invenManager.selectedButton = this.gameObject; // 선택된 버튼 설정

        GetComponent<CanvasGroup>().alpha = 0.5f; // 투명도 설정
    }

    public void SetUpButton(ItemClass passedItem, ItemListManager passedListManager) // 버튼을 설정하는 메서드
    {
        listManager = passedListManager; // 리스트 매니저 설정
        item = passedItem; // 아이템 설정
        ItemClass.SetItemValues(passedItem); // 아이템 값 설정
        nameText.text = passedItem.TypeName; // 아이템 이름 텍스트 설정
        LvlText.text = "Lvl: " + passedItem.Level.ToString(); // 아이템 레벨 텍스트 설정
        QualityText.text = passedItem.GetQualityStr(); // 아이템 품질 텍스트 설정
        GetComponent<LayoutElement>().preferredHeight = transform.parent.GetComponent<RectTransform>().rect.width / 4; // 레이아웃 요소 높이 설정
        iconImage.sprite = passedItem.Icon; // 아이템 아이콘 설정
        itemEquipPool = passedListManager.itemEquipPool; // 아이템 장비 오브젝트 풀 설정

        switch (item.qualityInt) // 품질에 따라 색상 설정
        {
            case 0: QualityColor.color = Color.gray; break;
            case 1: QualityColor.color = Color.white; break;
            case 2: QualityColor.color = new Color(0.5f, 0.5f, 1f, 1f); break;
            case 3: QualityColor.color = Color.yellow; break;
            default: break;
        }

        float iconSize = listManager.iconSize; // 아이콘 크기 설정
        RectTransform rect = iconImage.GetComponent<RectTransform>(); // 아이콘의 RectTransform 컴포넌트를 가져옴
        if (passedItem.Size.y >= passedItem.Size.x) // 아이템 크기에 따라 아이콘 크기 조정
        {
            rect.sizeDelta = new Vector2(iconSize / IntVector2.Slope(passedItem.Size), iconSize);
        }
        else
        {
            rect.sizeDelta = new Vector2(iconSize, iconSize * IntVector2.Slope(passedItem.Size));
        }
    }
}
