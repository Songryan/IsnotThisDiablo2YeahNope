using UnityEngine; // Unity 엔진 기능을 사용

public class InvenGridScript : MonoBehaviour // InvenGridScript 클래스 정의, MonoBehaviour를 상속
{
    /* 할 일 목록
     * 인벤토리 그리드 생성 (완료)
     * 패널 추가 (완료)
     * 동적 인벤토리 기능 (완료)
     * 
     * 테스트 아이템 생성 (완료)
     * 아이템 이동 (완료)
     * 아이템 드롭 (완료)
     * 아이템 가져오기 (완료)
     * 아이템 교환 (완료)
     * 드래그 확인 및 하이라이트 색상 (완료)
     * 색상 하이라이트 다시 쓰기 *너무 길고 읽기 어려움* (대체로 완료) *****
     * 
     * 아이템용 스크롤 리스트 UI 만들기 (완료)
     * 아이템 버튼 (완료)
     * 버튼에서 아이템 장비 생성 (완료)
     * 그리드에 아이템을 넣을 때 리스트에서 버튼 제거 (완료)
     * 버튼 오브젝트 풀 | 버튼 및 아이템 장비 (완료)
     * 리스트로 아이템 다시 드롭 (완료)
     * 아이템 삭제 패널 추가 (완료)
     * 
     * 아이템 통계 추가 **나중에
     * 아이템 통계 오버레이 추가 (완료 **다듬기 필요)
     * 
     * 전체 아이템 클래스 상속 "무기, 갑옷" 만들기
     * 더 많은 통계를 추가할 때 StatPanel 동적 크기 설정
     * 더 많은 아이템 유형, 이름 및 아이콘 추가 (완료)
     * 
     * 아이템 통계가 플레이어 통계에 영향을 미치도록 하기 *나중에*
     * 아이템 생성기 만들기 (완료)
     * 랜덤 아이템 생성기 만들기 (대체로 완료) **아이템 클래스 및 통계 확장 후 추가 작업 필요
     * 선택된 아이템이 없을 때 그리드의 아이템을 녹색으로 빛나게 하기 (완료)
     * 리스트에 프리셋 아이템 목록을 추가하는 버튼 만들기
     * 
     * 정렬 리스트 추가 (완료)
     * 정렬과 관련하여 리스트에 아이템 추가 다시 작업 (완료)
     */

    /* 선택 사항
     * 품질에 따라 배경색 변경 대신 텍스트 변경 ***나중에 색상 변경을 함수로 만들기
     * 특이한 모양의 아이템 생성 *매우 어려움, 전체 시스템 재작성 필요*
     * 그래픽 추가
     * 아이템 회전
     * 고품질 아이템 삭제 시 경고 팝업 추가
     * 저장/로드 기능 추가 *어려움/지식 부족*
     * IntVector2 메서드 및 매개변수 개선 *진행 중*
     * 그리드 정렬 추가 *어려움*
     */

    public GameObject[,] slotGrid; // 슬롯 그리드를 저장할 2차원 배열
    public GameObject slotPrefab; // 슬롯 프리팹을 저장할 변수
    public IntVector2 gridSize; // 그리드 크기를 저장할 변수
    public float slotSize; // 슬롯 크기를 저장할 변수
    public float edgePadding; // 가장자리 여백을 저장할 변수

    public void Awake() // Unity에서 스크립트가 깨어날 때 호출되는 메서드
    {
        slotGrid = new GameObject[gridSize.x, gridSize.y]; // 그리드 크기에 따라 슬롯 그리드 배열 초기화
        ResizePanel(); // 패널 크기 조정
        CreateSlots(); // 슬롯 생성
        GetComponent<InvenGridManager>().gridSize = gridSize; // 인벤토리 그리드 매니저의 그리드 크기 설정
    }

    private void CreateSlots() // 슬롯을 생성하는 메서드
    {
        for (int y = 0; y < gridSize.y; y++) // 그리드의 y축을 순회하며
        {
            for (int x = 0; x < gridSize.x; x++) // 그리드의 x축을 순회하며
            {
                GameObject obj = (GameObject)Instantiate(slotPrefab); // 슬롯 프리팹을 인스턴스화

                obj.transform.name = "slot[" + x + "," + y + "]"; // 슬롯 이름 설정
                obj.transform.SetParent(this.transform); // 슬롯의 부모를 현재 트랜스폼으로 설정
                RectTransform rect = obj.transform.GetComponent<RectTransform>(); // 슬롯의 RectTransform 컴포넌트를 가져옴
                rect.localPosition = new Vector3(x * slotSize + edgePadding, y * slotSize + edgePadding, 0); // 슬롯의 위치 설정
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize); // 슬롯의 가로 크기 설정
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize); // 슬롯의 세로 크기 설정
                obj.GetComponent<RectTransform>().localScale = Vector3.one; // 슬롯의 스케일 설정
                obj.GetComponent<SlotScript>().gridPos = new IntVector2(x, y); // 슬롯의 그리드 위치 설정
                slotGrid[x, y] = obj; // 슬롯을 슬롯 그리드 배열에 추가
            }
        }
        GetComponent<InvenGridManager>().slotGrid = slotGrid; // 인벤토리 그리드 매니저의 슬롯 그리드 설정
    }

    private void ResizePanel() // 패널 크기를 조정하는 메서드
    {
        float width, height; // 패널의 너비와 높이를 저장할 변수
        width = (gridSize.x * slotSize) + (edgePadding * 2); // 그리드 크기에 따라 너비 계산
        height = (gridSize.y * slotSize) + (edgePadding * 2); // 그리드 크기에 따라 높이 계산

        RectTransform rect = GetComponent<RectTransform>(); // 현재 트랜스폼의 RectTransform 컴포넌트를 가져옴
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width); // 패널의 가로 크기 설정
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height); // 패널의 세로 크기 설정
        //rect.localScale = Vector3.one; // 패널의 스케일 설정
        rect.localScale = new Vector3(0.32f, 0.32f, 0.32f); // 패널의 스케일 설정
    }
}
