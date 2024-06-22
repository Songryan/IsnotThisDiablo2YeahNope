using UnityEngine; // Unity 엔진 기능을 사용
using UnityEngine.EventSystems; // 이벤트 시스템을 사용
using UnityEngine.UI; // UI 요소를 사용

public class ItemScript : MonoBehaviour, IPointerClickHandler // ItemScript 클래스 정의, MonoBehaviour와 IPointerClickHandler를 상속
{
    private GameObject invenPanel; // 인벤토리 패널을 저장할 변수
    public static GameObject selectedItem; // 선택된 아이템을 저장할 정적 변수
    public static IntVector2 selectedItemSize; // 선택된 아이템의 크기를 저장할 정적 변수
    public static bool isDragging = false; // 아이템이 드래그 중인지 여부를 저장할 정적 변수

    private float slotSize; // 슬롯 크기를 저장할 변수

    public ItemClass item; // 아이템 클래스 인스턴스를 저장할 변수

    private void Awake() // Unity에서 스크립트가 깨어날 때 호출되는 메서드
    {
        slotSize = GameObject.FindGameObjectWithTag("InvenPanel").GetComponent<InvenGridScript>().slotSize; // "InvenPanel" 태그를 가진 오브젝트에서 슬롯 크기를 가져옴
    }

    public void SetItemObject(ItemClass passedItem) // 아이템 오브젝트를 설정하는 메서드
    {
        RectTransform rect = GetComponent<RectTransform>(); // 현재 오브젝트의 RectTransform 컴포넌트를 가져옴
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, passedItem.Size.x * slotSize); // 가로 크기 설정
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, passedItem.Size.y * slotSize); // 세로 크기 설정
        item = passedItem; // 아이템 설정
        GetComponent<Image>().sprite = passedItem.Icon; // 아이템 아이콘 설정
    }

    // obsolete
    public void OnPointerClick(PointerEventData eventData) // 마우스 클릭 이벤트 처리 메서드
    {
        SetSelectedItem(this.gameObject); // 선택된 아이템 설정
        CanvasGroup canvas = GetComponent<CanvasGroup>(); // CanvasGroup 컴포넌트를 가져옴
        canvas.blocksRaycasts = false; // 레이캐스트 차단 해제
        canvas.alpha = 0.5f; // 투명도 설정
    }

    private void Update() // Unity에서 매 프레임마다 호출되는 메서드
    {
        if (isDragging) // 아이템이 드래그 중인 경우
        {
            selectedItem.transform.position = Input.mousePosition; // 선택된 아이템의 위치를 마우스 위치로 설정
        }
    }

    public static void SetSelectedItem(GameObject obj) // 선택된 아이템을 설정하는 정적 메서드
    {
        selectedItem = obj; // 선택된 아이템 설정
        selectedItemSize = obj.GetComponent<ItemScript>().item.Size; // 선택된 아이템 크기 설정
        isDragging = true; // 드래그 중으로 설정
        obj.transform.SetParent(GameObject.FindGameObjectWithTag("DragParent").transform); // 부모를 "DragParent" 태그를 가진 오브젝트로 설정
        obj.GetComponent<RectTransform>().localScale = Vector3.one; // 크기 설정
    }

    public static void ResetSelectedItem() // 선택된 아이템을 초기화하는 정적 메서드
    {
        selectedItem = null; // 선택된 아이템 초기화
        selectedItemSize = IntVector2.Zero; // 선택된 아이템 크기 초기화
        isDragging = false; // 드래그 중 여부 초기화
    }
}
