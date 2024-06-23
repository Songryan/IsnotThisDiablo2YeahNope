using UnityEngine; // Unity 엔진 기능을 사용

[System.Serializable] // 이 클래스가 직렬화 가능하도록 설정
public class ItemClass
{
    public int GlobalID; // 글로벌 아이템 ID
    [HideInInspector] public int CategoryID; // 카테고리 ID, 인스펙터에서 숨김
    [HideInInspector] public string CategoryName; // 카테고리 이름, 인스펙터에서 숨김
    [HideInInspector] public int TypeID; // 타입 ID, 인스펙터에서 숨김
    public string TypeName; // 타입 이름
    public int Level; // 아이템 레벨
    [Range(0, 3)] public int qualityInt; // 품질 값, 0에서 3 사이의 값
    [HideInInspector] public IntVector2 Size; // 아이템 크기, 인스펙터에서 숨김
    [HideInInspector] public Sprite Icon; // 아이템 아이콘, 인스펙터에서 숨김
    public string SerialID; // 아이템 시리얼 ID

    // 기본 생성자, 생성자를 명시적으로 정의하지 않으면 오류 발생 (이유는 불명)
    public ItemClass() { }

    // 아이템을 복사하여 새 아이템을 생성하는 생성자
    public ItemClass(ItemClass passedItem)
    {
        GlobalID = passedItem.GlobalID; // 글로벌 ID 복사
        Level = passedItem.Level; // 레벨 복사
        qualityInt = passedItem.qualityInt; // 품질 복사
    }

    // 품질 문자열을 반환하는 속성
    public string qualityStr
    {
        get
        {
            switch (qualityInt)
            {
                case 0: return "Broken";
                case 1: return "Normal";
                case 2: return "Magic";
                case 3: return "Rare";
                default: return null;
            }
        }
    }

    // 아이템 값을 설정하는 정적 메서드
    public static void SetItemValues(ItemClass item, int ID, int lvl, int quality)
    {
        item.GlobalID = ID; // 글로벌 ID 설정
        item.Level = lvl; // 레벨 설정
        item.qualityInt = quality; // 품질 설정
        GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>().PassItemData(ref item); // 아이템 데이터베이스에서 아이템 데이터 가져오기
    }

    // 아이템 값을 설정하는 또 다른 정적 메서드 (ID, 레벨, 품질 없이)
    public static void SetItemValues(ItemClass item)
    {
        GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>().PassItemData(ref item); // 아이템 데이터베이스에서 아이템 데이터 가져오기
    }
}
