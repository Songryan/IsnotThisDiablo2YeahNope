using System; // 기본 시스템 라이브러리를 사용
using System.Collections; // 컬렉션 인터페이스를 사용
using System.Collections.Generic; // 제네릭 컬렉션을 사용
using UnityEngine; // Unity 엔진 기능을 사용
using UnityEngine.UI; // UI 요소를 사용

public class TrashButtonScript : MonoBehaviour
{ // TrashButtonScript 클래스 정의, MonoBehaviour를 상속

    public Button button; // 버튼 컴포넌트를 저장할 변수
    public InvenGridManager invenManager; // 인벤토리 그리드 매니저를 저장할 변수
    public ItemListManager listManager; // 아이템 리스트 매니저를 저장할 변수

    private void Start() // Unity에서 스크립트가 처음 실행될 때 호출되는 메서드
    {
        button.onClick.AddListener(DestroyItem); // 버튼 클릭 이벤트에 DestroyItem 메서드를 리스너로 추가
    }

    private void DestroyItem() // 아이템을 파괴하는 메서드
    {
        if (ItemScript.selectedItem != null) // 선택된 아이템이 있는지 확인
        {
            invenManager.RemoveSelectedButton(); // 인벤토리 매니저에서 선택된 버튼 제거
            listManager.itemEquipPool.ReturnObject(ItemScript.selectedItem); // 아이템을 오브젝트 풀로 반환
            ItemScript.ResetSelectedItem(); // 선택된 아이템 초기화
        }
    }
}
