using System.Collections; // 컬렉션 인터페이스를 사용
using System.Collections.Generic; // 제네릭 컬렉션을 사용
using UnityEngine; // Unity 엔진 기능을 사용
using UnityEngine.UI; // UI 요소를 사용

public class AddPresetItemsScript : MonoBehaviour
{ // AddPresetItemsScript 클래스 정의, MonoBehaviour를 상속

    private List<ItemClass> itemsToAdd = new List<ItemClass>(); // 추가할 아이템 목록
    private Button button; // 버튼 컴포넌트를 저장할 변수

    public InvenGridManager gridManager; // 인벤토리 그리드 매니저를 저장할 변수
    public ItemListManager listManager; // 아이템 리스트 매니저를 저장할 변수
    public SortAndFilterManager sortManager; // 정렬 및 필터 매니저를 저장할 변수
    public Transform dropParent; // 아이템이 드롭될 부모 트랜스폼을 저장할 변수
    public List<GameObject> gridItems; // 그리드 아이템을 저장할 리스트

    private void Start() // Unity에서 스크립트가 처음 실행될 때 호출되는 메서드
    {
        for (int i = 0; i < 17; i++) // 17개의 아이템을 생성하여 itemsToAdd 리스트에 추가
        {
            ItemClass item = new ItemClass(); // 새로운 아이템 생성
            item.GlobalID = i; // 아이템의 GlobalID 설정
            item.Level = Random.Range(0, 101); // 아이템의 레벨을 0에서 100 사이의 랜덤 값으로 설정
            item.qualityInt = Random.Range(0, 4); // 아이템의 품질을 0에서 3 사이의 랜덤 값으로 설정
            ItemClass.SetItemValues(item); // 아이템 값 설정
            itemsToAdd.Add(item); // 아이템을 리스트에 추가
        }
        button = GetComponent<Button>(); // 버튼 컴포넌트를 가져옴
        button.onClick.AddListener(AddItems); // 버튼 클릭 리스너에 AddItems 메서드 추가
    }

    private void AddItems() // 아이템을 추가하는 메서드
    {
        listManager.currentItemList.AddRange(itemsToAdd); // 아이템 리스트에 itemsToAdd 리스트의 아이템들을 추가
        sortManager.SortAndFilterList(); // 리스트를 정렬 및 필터링
    }

    public void RemoveListButtons() // 리스트 버튼을 제거하는 메서드
    {
        listManager.currentItemList.Clear(); // 현재 아이템 리스트를 비움
        if (listManager.currentButtonList.Count > 0) // 현재 버튼 리스트가 비어 있지 않으면
        {
            for (int i = listManager.currentButtonList.Count - 1; i >= 0; i--) // 모든 버튼을 제거
            {
                listManager.RemoveButton(listManager.currentButtonList[i]); // 버튼 제거 메서드 호출
            }
        }
    }

    public void RemoveGridItems() // 그리드 아이템을 제거하는 메서드
    {
        foreach (Transform child in dropParent.transform) // 드롭 부모의 모든 자식 트랜스폼을 순회
        {
            gridItems.Add(child.gameObject); // 자식 게임 오브젝트를 gridItems 리스트에 추가
        }
        for (int i = gridItems.Count - 1; i >= 0; i--) // 모든 그리드 아이템을 제거
        {
            gridItems[i].GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f); // 피벗 설정
            listManager.itemEquipPool.ReturnObject(gridItems[i]); // 아이템을 오브젝트 풀에 반환
        }
        gridItems.Clear(); // gridItems 리스트를 비움

        // 모든 slotGrid 속성을 초기화
        IntVector2 gridSize = gridManager.gridSize; // 그리드 크기를 가져옴
        GameObject[,] slotGrid = gridManager.slotGrid; // 슬롯 그리드를 가져옴
        SlotScript instanceScript;
        for (int y = 0; y < gridSize.y; y++) // 그리드의 y축을 순회
        {
            for (int x = 0; x < gridSize.x; x++) // 그리드의 x축을 순회
            {
                slotGrid[x, y].GetComponent<Image>().color = SlotColorHighlights.Gray; // 슬롯의 색상을 회색으로 설정
                instanceScript = slotGrid[x, y].GetComponent<SlotScript>(); // 슬롯 스크립트를 가져옴
                instanceScript.storedItemObject = null; // 저장된 아이템 오브젝트 초기화
                instanceScript.storedItemClass = null; // 저장된 아이템 클래스 초기화
                instanceScript.storedItemSize = IntVector2.Zero; // 저장된 아이템 크기 초기화
                instanceScript.storedItemStartPos = IntVector2.Zero; // 저장된 아이템 시작 위치 초기화
                instanceScript.isOccupied = false; // 슬롯이 점유되지 않음으로 설정
            }
        }
    }
}
