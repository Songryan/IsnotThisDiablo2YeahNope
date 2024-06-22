using UnityEngine; // Unity 엔진 기능을 사용
using System.Collections.Generic; // 제네릭 컬렉션을 사용
using System; // 기본 시스템 라이브러리를 사용

public class ItemDatabase : MonoBehaviour // ItemDatabase 클래스 정의, MonoBehaviour를 상속
{
    public TextAsset file; // CSV 파일을 저장할 변수
    public List<int> GlobalIDList; // 글로벌 ID 리스트
    public List<string> TypeNameList; // 타입 이름 리스트
    public List<IntVector2> SizeList; // 크기 리스트
    public List<Sprite> IconList; // 아이콘 리스트

    private void Awake() // Unity에서 스크립트가 깨어날 때 호출되는 메서드
    {
        Load(file); // CSV 파일을 로드
    }

    public class Row // CSV 파일의 한 행을 나타내는 클래스
    {
        public int GlobalID; // 글로벌 ID
        public int CategoryID; // 카테고리 ID
        public string CategoryName; // 카테고리 이름
        public int TypeID; // 타입 ID
        public string TypeName; // 타입 이름
        public IntVector2 Size; // 크기
        public Sprite Icon; // 아이콘
        public string SerailID; // 시리얼 ID
    }

    public List<Row> rowList = new List<Row>(); // CSV 파일의 모든 행을 저장할 리스트
    bool isLoaded = false; // 파일이 로드되었는지 여부를 저장할 변수

    public bool IsLoaded() // 파일이 로드되었는지 확인하는 메서드
    {
        return isLoaded;
    }

    public List<Row> GetRowList() // 행 리스트를 반환하는 메서드
    {
        return rowList;
    }

    public void Load(TextAsset csv) // CSV 파일을 로드하는 메서드
    {
        rowList.Clear(); // 기존 리스트를 초기화
        string[][] grid = CsvParser2.Parse(csv.text); // CSV 파일을 파싱하여 2차원 배열로 변환
        for (int i = 1; i < grid.Length; i++) // 첫 번째 행을 제외하고 각 행을 순회
        {
            Row row = new Row(); // 새로운 행 객체 생성
            row.GlobalID = Int32.Parse(grid[i][0]); // 글로벌 ID 설정
            GlobalIDList.Add(row.GlobalID); // 글로벌 ID 리스트에 추가
            row.CategoryID = Int32.Parse(grid[i][1]); // 카테고리 ID 설정
            row.CategoryName = grid[i][2]; // 카테고리 이름 설정
            row.TypeID = Int32.Parse(grid[i][3]); // 타입 ID 설정
            row.TypeName = grid[i][4]; // 타입 이름 설정
            TypeNameList.Add(row.TypeName); // 타입 이름 리스트에 추가
            row.Size = new IntVector2(Int32.Parse(grid[i][5]), Int32.Parse(grid[i][6])); // 크기 설정
            SizeList.Add(row.Size); // 크기 리스트에 추가
            row.Icon = Resources.Load<Sprite>("ItemIcons/" + grid[i][4]); // 아이콘 로드
            IconList.Add(row.Icon); // 아이콘 리스트에 추가
            row.SerailID = grid[i][7]; // 시리얼 ID 설정
            rowList.Add(row); // 행 리스트에 추가
        }
        isLoaded = true; // 파일 로드 완료
    }

    public void PassItemData(ref ItemClass item) // 아이템 데이터를 전달하는 메서드
    {
        int ID = item.GlobalID; // 글로벌 ID를 가져옴
        item.CategoryID = rowList[ID].CategoryID; // 카테고리 ID 설정
        item.CategoryName = rowList[ID].CategoryName; // 카테고리 이름 설정
        item.TypeID = rowList[ID].TypeID; // 타입 ID 설정
        item.TypeName = rowList[ID].TypeName; // 타입 이름 설정
        item.Size = rowList[ID].Size; // 크기 설정
        item.Icon = rowList[ID].Icon; // 아이콘 설정
        item.SerialID = rowList[ID].SerailID; // 시리얼 ID 설정
    }

    public int NumRows() // 행의 수를 반환하는 메서드
    {
        return rowList.Count;
    }

    public Row GetAt(int i) // 특정 인덱스의 행을 반환하는 메서드
    {
        if (rowList.Count <= i)
            return null; // 인덱스가 범위를 벗어나면 null 반환
        return rowList[i]; // 해당 인덱스의 행 반환
    }
}
