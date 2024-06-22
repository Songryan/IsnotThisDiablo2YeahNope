using System; // 기본 시스템 라이브러리를 사용
using System.Collections; // 컬렉션 인터페이스를 사용
using System.Collections.Generic; // 제네릭 컬렉션을 사용
using UnityEngine; // Unity 엔진 기능을 사용
using UnityEngine.EventSystems; // 이벤트 시스템을 사용

public class SlotSectorScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler // SlotSectorScript 클래스 정의, MonoBehaviour와 인터페이스를 상속
{
    public GameObject slotParent; // 슬롯의 부모 게임 오브젝트를 저장할 변수
    public int QuadNum; // 사분면 번호를 저장할 변수
    public static IntVector2 posOffset; // 위치 오프셋을 저장할 정적 변수
    public static SlotSectorScript sectorScript; // 현재 섹터 스크립트를 저장할 정적 변수
    public static ItemOverlayScript overlayScript; // 아이템 오버레이 스크립트를 저장할 정적 변수
    private InvenGridManager invenGridManager; // 인벤토리 그리드 매니저를 저장할 변수
    private SlotScript parentSlotScript; // 부모 슬롯 스크립트를 저장할 변수

    private void Start() // Unity에서 스크립트가 처음 실행될 때 호출되는 메서드
    {
        invenGridManager = this.gameObject.transform.parent.parent.GetComponent<InvenGridManager>(); // 부모의 부모 오브젝트에서 InvenGridManager 컴포넌트를 가져옴
        parentSlotScript = slotParent.GetComponent<SlotScript>(); // 부모 슬롯 오브젝트에서 SlotScript 컴포넌트를 가져옴
    }

    public void OnPointerEnter(PointerEventData eventData) // 마우스가 슬롯에 진입할 때 호출되는 메서드
    {
        sectorScript = this; // 현재 섹터 스크립트를 설정
        invenGridManager.highlightedSlot = slotParent; // 강조된 슬롯을 부모 슬롯으로 설정
        PosOffset(); // 위치 오프셋 설정
        if (ItemScript.selectedItem != null) // 선택된 아이템이 있을 경우
        {
            invenGridManager.RefrechColor(true); // 색상 갱신
        }
        if (parentSlotScript.storedItemObject != null && ItemScript.selectedItem == null) // 부모 슬롯에 아이템이 저장되어 있고 선택된 아이템이 없을 경우
        {
            invenGridManager.ColorChangeLoop(SlotColorHighlights.Blue, parentSlotScript.storedItemSize, parentSlotScript.storedItemStartPos); // 슬롯 색상 변경
        }
        if (parentSlotScript.storedItemObject != null) // 부모 슬롯에 아이템이 저장되어 있을 경우
        {
            overlayScript.UpdateOverlay(parentSlotScript.storedItemClass); // 오버레이를 아이템 정보로 업데이트
        }
    }

    public void PosOffset() // 위치 오프셋을 설정하는 메서드
    {
        if (ItemScript.selectedItemSize.x != 0 && ItemScript.selectedItemSize.x % 2 == 0) // 선택된 아이템의 가로 크기가 짝수인 경우
        {
            switch (QuadNum) // 사분면 번호에 따라 오프셋 설정
            {
                case 1:
                    posOffset.x = 0; break;
                case 2:
                    posOffset.x = -1; break;
                case 3:
                    posOffset.x = 0; break;
                case 4:
                    posOffset.x = -1; break;
                default: break;
            }
        }
        if (ItemScript.selectedItemSize.y != 0 && ItemScript.selectedItemSize.y % 2 == 0) // 선택된 아이템의 세로 크기가 짝수인 경우
        {
            switch (QuadNum) // 사분면 번호에 따라 오프셋 설정
            {
                case 1:
                    posOffset.y = -1; break;
                case 2:
                    posOffset.y = -1; break;
                case 3:
                    posOffset.y = 0; break;
                case 4:
                    posOffset.y = 0; break;
                default: break;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) // 마우스가 슬롯에서 나갈 때 호출되는 메서드
    {
        sectorScript = null; // 현재 섹터 스크립트를 초기화
        invenGridManager.highlightedSlot = null; // 강조된 슬롯을 초기화
        overlayScript.UpdateOverlay(null); // 오버레이를 초기화
        if (ItemScript.selectedItem != null) // 선택된 아이템이 있을 경우
        {
            invenGridManager.RefrechColor(false); // 색상 갱신
        }
        posOffset = IntVector2.Zero; // 위치 오프셋 초기화
        if (parentSlotScript.storedItemObject != null && ItemScript.selectedItem == null) // 부모 슬롯에 아이템이 저장되어 있고 선택된 아이템이 없을 경우
        {
            invenGridManager.ColorChangeLoop(SlotColorHighlights.Blue2, parentSlotScript.storedItemSize, parentSlotScript.storedItemStartPos); // 슬롯 색상 변경
        }
    }
}
