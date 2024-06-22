using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SortAndFilterManager : MonoBehaviour
{

    public ItemListManager listManager; // 아이템 리스트를 관리하는 매니저
    private List<ItemClass> itemList; // 아이템 리스트

    public List<GameObject> categoryButtons; // 카테고리 필터 버튼 목록
    private GameObject selectedCatButton; // 선택된 카테고리 버튼
    public int catFilterInt = 0; // 카테고리 필터 인덱스

    public Image qualityButtonImage; // 품질 필터 버튼 이미지
    public Text qualityButtonText; // 품질 필터 버튼 텍스트
    private int qualityFilterInt = 0; // 품질 필터 인덱스

    private List<ItemClass> filteredList; // 필터링된 아이템 리스트
    public List<ItemClass> sortedList; // 정렬된 아이템 리스트
    private int sortTypeInt = 0; // 정렬 타입 인덱스

    private void Start()
    {
        selectedCatButton = categoryButtons[0]; // 기본 선택된 카테고리 버튼을 첫 번째 버튼으로 설정
        qualityButtonImage.color = Color.gray; // 품질 필터 버튼의 초기 색상을 회색으로 설정
        sortedList = SortList(listManager.startItemList); // 시작 아이템 리스트를 정렬하여 sortedList에 저장
        listManager.currentItemList = sortedList; // 현재 아이템 리스트를 정렬된 리스트로 설정
        listManager.PopulateList(sortedList); // 리스트 매니저에서 리스트를 갱신
        filteredList = sortedList; // 필터링된 리스트를 정렬된 리스트로 초기화
    }

    #region filter list
    private void FilterList(List<ItemClass> list)
    {
        list = FilterByClass(list); // 카테고리로 필터링
        list = FilterByQuality(list); // 품질로 필터링
        filteredList = list; // 필터링된 리스트를 저장
        listManager.PopulateList(filteredList); // 리스트 매니저에서 리스트를 갱신
    }

    private void ClassFilterChange(int type) // 카테고리 필터 버튼이 변경될 때 호출
    {
        if (selectedCatButton != categoryButtons[type]) // 선택된 카테고리 버튼이 변경되었는지 확인
        {
            catFilterInt = type; // 카테고리 필터 인덱스를 갱신
            categoryButtons[type].GetComponent<CanvasGroup>().alpha = 1f; // 새로운 선택된 버튼의 투명도를 1로 설정
            selectedCatButton.GetComponent<CanvasGroup>().alpha = 0.5f; // 이전 선택된 버튼의 투명도를 0.5로 설정
            selectedCatButton = categoryButtons[type]; // 선택된 버튼을 갱신
            FilterList(sortedList); // 필터 리스트 갱신
        }
    }

    private List<ItemClass> FilterByClass(List<ItemClass> list) // 카테고리 필터링 메서드
    {
        if (catFilterInt == 0) return list; // 필터 인덱스가 0이면 필터링하지 않음
        else return list.FindAll(x => x.CategoryID == catFilterInt - 1); // 카테고리 ID가 필터 인덱스와 일치하는 아이템을 반환
    }

    private List<ItemClass> FilterByQuality(List<ItemClass> list) // 품질 필터링 메서드
    {
        if (qualityFilterInt == 0) return list; // 품질 필터 인덱스가 0이면 필터링하지 않음
        else return list.FindAll(x => x.qualityInt >= qualityFilterInt); // 품질 인덱스 이상인 아이템을 반환
    }

    private void QualityButtonClick() // 품질 필터 버튼이 클릭될 때 호출
    {
        switch (qualityFilterInt) // 품질 필터 인덱스에 따라 버튼 텍스트 변경
        {
            case 0: qualityFilterInt = 1; qualityButtonText.text = "Normal+"; break;
            case 1: qualityFilterInt = 2; qualityButtonText.text = "Magic+"; break;
            case 2: qualityFilterInt = 3; qualityButtonText.text = "Rare"; break;
            case 3: qualityFilterInt = 0; qualityButtonText.text = "All"; break;
            default: break;
        }
        switch (qualityFilterInt) // 품질 필터 인덱스에 따라 버튼 색상 변경
        {
            case 0: qualityButtonImage.color = Color.gray; break;
            case 1: qualityButtonImage.color = Color.white; break;
            case 2: qualityButtonImage.color = new Color(0.5f, 0.5f, 1f, 1f); break;
            case 3: qualityButtonImage.color = Color.yellow; break;
            default: break;
        }
        FilterList(sortedList); // 필터 리스트 갱신
    }
    #endregion

    #region sortlist
    public void OnSortTypeChange(int index) // 정렬 타입 드롭다운이 변경될 때 호출
    {
        sortTypeInt = index; // 정렬 타입 인덱스를 갱신
        sortedList = SortList(listManager.currentItemList); // 현재 아이템 리스트를 정렬하여 저장
        listManager.currentItemList = sortedList; // 현재 아이템 리스트 갱신
        filteredList = SortList(filteredList); // 필터링된 리스트를 정렬하여 저장
        listManager.PopulateList(filteredList); // 리스트 매니저에서 리스트를 갱신
    }

    public List<ItemClass> SortList(List<ItemClass> list) // 정렬 메서드
    {
        switch (sortTypeInt) // 정렬 타입 인덱스에 따라 정렬 방식 결정
        {
            case 0: return list.OrderBy(x => x.GlobalID).ToList(); // GlobalID 오름차순 정렬
            case 1: return list.OrderByDescending(x => x.GlobalID).ToList(); // GlobalID 내림차순 정렬
            case 2: return list.OrderBy(x => x.Level).ToList(); // Level 오름차순 정렬
            case 3: return list.OrderByDescending(x => x.Level).ToList(); // Level 내림차순 정렬
            case 4: return list.OrderBy(x => x.qualityInt).ToList(); // qualityInt 오름차순 정렬
            case 5: return list.OrderByDescending(x => x.qualityInt).ToList(); // qualityInt 내림차순 정렬
            case 6: return list.OrderBy(x => x.TypeName).ToList(); // TypeName 오름차순 정렬
            case 7: return list.OrderByDescending(x => x.TypeName).ToList(); // TypeName 내림차순 정렬
            default: return list; // 기본값은 변경 없이 리스트 반환
        }
    }
    #endregion

    public void AddItemToList(ItemClass item) // 아이템을 리스트에 추가하는 메서드
    {
        listManager.currentItemList.Add(item); // 현재 아이템 리스트에 아이템 추가
        SortAndFilterList(); // 정렬 및 필터 리스트 갱신
    }

    public void SortAndFilterList() // 정렬 및 필터링을 수행하는 메서드
    {
        sortedList = SortList(listManager.currentItemList); // 현재 아이템 리스트를 정렬하여 저장
        listManager.currentItemList = sortedList; // 현재 아이템 리스트 갱신
        FilterList(sortedList); // 정렬된 리스트를 필터링
        listManager.PopulateList(filteredList); // 리스트 매니저에서 리스트를 갱신
    }
}
