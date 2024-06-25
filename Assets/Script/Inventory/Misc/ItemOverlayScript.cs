using UnityEngine; // Unity 엔진 기능을 사용
using UnityEngine.UI; // UI 요소를 사용

public class ItemOverlayScript : MonoBehaviour
{ // ItemOverlayScript 클래스 정의, MonoBehaviour를 상속

    public Text nameText, lvlText, qualityText, size; // 아이템 이름, 레벨, 품질, 크기를 표시할 텍스트 UI
    public Image Icon; // 아이템 아이콘을 표시할 이미지 UI
    public Sprite nullSprite; // 아이템이 없을 때 표시할 스프라이트

    public Text Str;
    public Text Dex;
    public Text Vital;
    public Text Mana;

    private float slotSize; // 슬롯 크기

    private void Start() // Unity에서 스크립트가 처음 실행될 때 호출되는 메서드
    {
        ItemButtonScript.overlayScript = this; // ItemButtonScript의 overlayScript 변수에 현재 인스턴스를 설정
        SlotSectorScript.overlayScript = this; // SlotSectorScript의 overlayScript 변수에 현재 인스턴스를 설정
        UpdateOverlay(null); // 초기 오버레이 업데이트(null 값으로 초기화)
        slotSize = GameObject.FindGameObjectWithTag("InvenPanel").GetComponent<InvenGridScript>().slotSize; // "InvenPanel" 태그를 가진 게임 오브젝트에서 슬롯 크기를 가져옴
    }

    public void UpdateOverlay(ItemClass item) // 아이템 정보를 오버레이에 업데이트하는 메서드
    {
        if (item != null) // 아이템이 null이 아닐 경우
        {
            nameText.text = item.TypeName; // 아이템 이름 업데이트
            lvlText.text = "Lvl: " + item.Level; // 아이템 레벨 업데이트
            qualityText.text = item.GetQualityStr(); // 아이템 품질 업데이트

            Str.text = $"Str : +{item.Str.ToString()}";
            Dex.text = $"Dex : +{item.Dex.ToString()}";
            Vital.text = $"Vital : +{item.Vital.ToString()}";
            Mana.text = $"Mana : +{item.Mana.ToString()}";

            switch (item.qualityInt) // 품질에 따라 텍스트 색상 변경
            {
                case 0: qualityText.color = Color.gray; break; // 품질이 0이면 회색
                case 1: qualityText.color = Color.white; break; // 품질이 1이면 흰색
                case 2: qualityText.color = new Color(0.5f, 0.5f, 1f, 1f); break; // 품질이 2이면 파란색
                case 3: qualityText.color = Color.yellow; break; // 품질이 3이면 노란색
                default: break;
            }
            Icon.color = new Color32(255, 255, 255, 255); // 아이콘 색상을 흰색으로 설정
            Icon.sprite = item.Icon; // 아이콘 스프라이트 업데이트
            size.text = item.Size.String; // 아이템 크기 업데이트
            RectTransform rect = Icon.GetComponent<RectTransform>(); // 아이콘의 RectTransform 컴포넌트를 가져옴
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, item.Size.x * slotSize); // 아이콘의 가로 크기 설정
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, item.Size.y * slotSize); // 아이콘의 세로 크기 설정
        }
        else // 아이템이 null일 경우
        {
            nameText.text = null; // 이름 텍스트 초기화
            lvlText.text = null; // 레벨 텍스트 초기화
            qualityText.text = null; // 품질 텍스트 초기화
            size.text = IntVector2.Zero.String; // 크기 텍스트 초기화
            Icon.sprite = null; // 아이콘 스프라이트 초기화
            Icon.color = new Color32(0, 0, 0, 0); // 아이콘 색상을 투명으로 설정
        }
    }

    public void UpdateOverlay2(ItemClass item, bool ID, bool lvl, bool quality) // 두 번째 오버레이 업데이트 메서드
    {
        if (item != null) // 아이템이 null이 아닐 경우
        {
            nameText.text = ID ? item.TypeName : "***"; // ID가 true이면 아이템 이름, 그렇지 않으면 "***"
            lvlText.text = lvl ? "Lvl: " + item.Level : "***"; // lvl이 true이면 아이템 레벨, 그렇지 않으면 "***"
            qualityText.text = quality ? item.GetQualityStr() : "***"; // quality가 true이면 아이템 품질, 그렇지 않으면 "***"
            Icon.color = new Color32(255, 255, 255, 255); // 아이콘 색상을 흰색으로 설정
            Icon.sprite = ID ? item.Icon : nullSprite; // ID가 true이면 아이콘, 그렇지 않으면 nullSprite
            switch (item.qualityInt) // 품질에 따라 텍스트 색상 변경
            {
                case 0: qualityText.color = Color.gray; break; // 품질이 0이면 회색
                case 1: qualityText.color = Color.white; break; // 품질이 1이면 흰색
                case 2: qualityText.color = new Color(0.5f, 0.5f, 1f, 1f); break; // 품질이 2이면 파란색
                case 3: qualityText.color = Color.yellow; break; // 품질이 3이면 노란색
                default: break;
            }
            IntVector2 size = ID ? item.Size : new IntVector2(4, 4); // ID가 true이면 아이템 크기, 그렇지 않으면 (4,4)
            RectTransform rect = Icon.GetComponent<RectTransform>(); // 아이콘의 RectTransform 컴포넌트를 가져옴
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x * slotSize); // 아이콘의 가로 크기 설정
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y * slotSize); // 아이콘의 세로 크기 설정
        }
        else // 아이템이 null일 경우
        {
            nameText.text = null; // 이름 텍스트 초기화
            lvlText.text = null; // 레벨 텍스트 초기화
            qualityText.text = null; // 품질 텍스트 초기화
            size.text = IntVector2.Zero.String; // 크기 텍스트 초기화
            Icon.sprite = null; // 아이콘 스프라이트 초기화
            Icon.color = new Color32(0, 0, 0, 0); // 아이콘 색상을 투명으로 설정
        }
    }
}
