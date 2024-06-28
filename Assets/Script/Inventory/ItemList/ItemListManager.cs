using System.Collections.Generic; // 제네릭 컬렉션을 사용
using UnityEngine; // Unity 엔진 기능을 사용

public class ItemListManager : MonoBehaviour
{ // ItemListManager 클래스 정의, MonoBehaviour를 상속
    /***** NEEDS AN OVEHAUL *****/
    public ObjectPoolScript itemButtonPool; // 아이템 버튼을 관리하는 오브젝트 풀
    public ObjectPoolScript itemEquipPool; // 아이템 장비를 관리하는 오브젝트 풀
    public InvenGridManager invenManager; // 인벤토리 그리드 매니저
    public ItemDatabase itemDatabase; // 아이템 데이터베이스
    public SortAndFilterManager sortManager; // 정렬 및 필터 매니저

    public float iconSize; // 아이콘 크기

    public List<ItemClass> startItemList; // 초기 아이템 리스트
    public List<GameObject> currentButtonList; // 현재 버튼 리스트
    public List<ItemClass> currentItemList; // 현재 아이템 리스트

    [SerializeField] private Transform contentPanel; // 콘텐츠 패널

    private void Start() // Unity에서 스크립트가 처음 실행될 때 호출되는 메서드
    {
        //contentPanel = this.transform; // 현재 트랜스폼을 콘텐츠 패널로 설정
        for (int i = 0; i < startItemList.Count; i++) // 초기 아이템 리스트의 각 아이템을 설정
        {
            ItemClass.SetItemValues(startItemList[i]); // 아이템 값을 설정
        }
        // 리스트는 SortAndFilterManager에서 초기화됩니다
    }

    //*** rework the add item to list
    // make a proper add item to list with sort and filter in mind

    private void Update() // Unity에서 매 프레임마다 호출되는 메서드
    {
        if (Input.GetMouseButtonDown(1) && invenManager.selectedButton != null) // 오른쪽 클릭으로 리스트에서 아이템을 반환
        {
            invenManager.RefrechColor(false); // 색상 갱신
            invenManager.selectedButton.GetComponent<CanvasGroup>().alpha = 1f; // 선택된 버튼의 투명도 설정
            invenManager.selectedButton = null; // 선택된 버튼 초기화
            itemEquipPool.ReturnObject(ItemScript.selectedItem); // 선택된 아이템을 오브젝트 풀에 반환
            ItemScript.ResetSelectedItem(); // 선택된 아이템 초기화
        }
    }

    public void AddSelectedItemToList() // 스크롤뷰에서 포인터 클릭으로 아이템을 리스트에 추가
    {
        if (invenManager.selectedButton == null && ItemScript.selectedItem != null) // 선택된 버튼이 없고 선택된 아이템이 있을 경우
        {
            // Json에 데이터추가
            JsonDataManager.Instance.AddItemJsonData(ItemScript.selectedItem.transform.GetComponent<ItemScript>().item);
            ItemClass item = ItemScript.selectedItem.GetComponent<ItemScript>().item; // 선택된 아이템을 가져옴
            sortManager.AddItemToList(item); // 아이템을 리스트에 추가
            itemEquipPool.ReturnObject(ItemScript.selectedItem); // 선택된 아이템을 오브젝트 풀에 반환
            ItemScript.ResetSelectedItem(); // 선택된 아이템 초기화
        }
    }

    public void PopulateList(List<ItemClass> passedItemlist) // 리스트를 채우는 메서드
    {
        if (currentButtonList.Count > 0) // 현재 버튼 리스트에 아이템이 있을 경우
        {
            for (int i = currentButtonList.Count - 1; i >= 0; i--) // 모든 버튼을 제거
            {
                RemoveButton(currentButtonList[i]); // 버튼 제거 메서드 호출
            }
        }
        for (int j = 0; j < passedItemlist.Count; j++) // 전달된 아이템 리스트를 순회하며
        {
            AddButton(passedItemlist[j]); // 버튼을 추가
        }
    }

    public void AddButton(ItemClass addItem) // 버튼을 추가하는 메서드
    {
        GameObject newButton = itemButtonPool.GetObject(); // 오브젝트 풀에서 새 버튼을 가져옴
        newButton.transform.SetParent(contentPanel); // 버튼의 부모를 콘텐츠 패널로 설정
        newButton.GetComponent<RectTransform>().localScale = Vector3.one; // 버튼의 크기 설정
        newButton.GetComponent<ItemButtonScript>().SetUpButton(addItem, this); // 버튼을 설정
        currentButtonList.Add(newButton); // 현재 버튼 리스트에 추가
    }

    public void ForInvenAddButton(ItemClass addItem) // 버튼을 추가하는 메서드
    {
        GameObject newButton = itemButtonPool.GetObject(); // 오브젝트 풀에서 새 버튼을 가져옴
        newButton.transform.SetParent(contentPanel); // 버튼의 부모를 콘텐츠 패널로 설정
        newButton.GetComponent<RectTransform>().localScale = Vector3.one; // 버튼의 크기 설정
        newButton.GetComponent<ItemButtonScript>().SetUpButton(addItem, this); // 버튼을 설정
        JsonDataManager.Instance.InvenButtonList.Add(newButton); // 현재 버튼 리스트에 추가
    }
    // List에서 넘어올때 Dictionary에 저장하는 Object 생성 메소드
    public GameObject ForInvenDataBtnObjMaker(ItemClass addItem) // 버튼을 추가하는 메서드
    {
        GameObject newButton = itemButtonPool.GetObject(); // 오브젝트 풀에서 새 버튼을 가져옴
        newButton.transform.SetParent(contentPanel); // 버튼의 부모를 콘텐츠 패널로 설정
        newButton.GetComponent<RectTransform>().localScale = Vector3.one; // 버튼의 크기 설정
        newButton.GetComponent<ItemButtonScript>().SetUpButton(addItem, this); // 버튼을 설정
        return newButton; // 현재 버튼 리스트에 추가
    }

    public void RemoveButton(GameObject buttonObj) // 버튼을 제거하는 메서드
    {
        buttonObj.GetComponent<CanvasGroup>().alpha = 1f; // 버튼의 투명도 설정
        currentButtonList.Remove(buttonObj); // 현재 버튼 리스트에서 제거
        itemButtonPool.ReturnObject(buttonObj); // 오브젝트 풀에 반환
    }

    public void RevomeItemFromList(ItemClass itemToRemove) // 그리드에 아이템을 배치하거나 삭제할 때 리스트에서 아이템을 제거
    {
        for (int i = currentItemList.Count - 1; i >= 0; i--) // 현재 아이템 리스트를 역순으로 순회하며
        {
            if (currentItemList[i] == itemToRemove) // 아이템이 리스트에 있을 경우
            {
                currentItemList.RemoveAt(i); // 아이템을 제거
                break; // 임시로 설정 (추후 수정 가능)
            }
        }
    }
}
