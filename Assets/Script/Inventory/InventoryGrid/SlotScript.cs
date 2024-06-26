using System.Collections; // 컬렉션 인터페이스를 사용
using System.Collections.Generic; // 제네릭 컬렉션을 사용
using Unity.VisualScripting;
using UnityEngine; // Unity 엔진 기능을 사용
using UnityEngine.UI; // UI 요소를 사용

public class SlotScript : MonoBehaviour // SlotScript 클래스 정의, MonoBehaviour를 상속
{
    public IntVector2 gridPos; // 그리드에서 슬롯의 위치를 저장할 변수
    public Text text; // 슬롯의 텍스트 UI 요소를 저장할 변수

    public GameObject storedItemObject; // 슬롯에 저장된 아이템 오브젝트를 저장할 변수
    public IntVector2 storedItemSize; // 저장된 아이템의 크기를 저장할 변수
    public IntVector2 storedItemStartPos; // 저장된 아이템의 시작 위치를 저장할 변수
    public ItemClass storedItemClass; // 저장된 아이템 클래스를 저장할 변수
    public bool isOccupied; // 슬롯이 점유되었는지 여부를 저장할 변수

    public string invenType;

    public InvenGridManager gridManager;

    private void Start() // Unity에서 스크립트가 처음 실행될 때 호출되는 메서드
    {
        text.text = gridPos.x + "," + gridPos.y; // 슬롯의 텍스트를 그리드 위치로 설정
        invenType = transform.parent.GetComponent<InvenGridScript>().slotType;
        gridManager = GameObject.Find("InvenPanel").transform.GetComponent<InvenGridManager>();
    }

    public void EquipInvenColorChanger()
    {
        if (invenType.Equals("All") == true)
            return;

        if(invenType.Equals("All") == false)
        {
            transform.GetComponent<Image>().color = new Color32(0,0,0,0);
        }

        if (invenType.Equals("All") == false && isOccupied)
        {
            //bool placeholderBool = transform.parent.GetComponent<InvenGridManager>().placeholder.gameObject.activeSelf;
            //bool highlighterBool = transform.parent.GetComponent<InvenGridManager>().highlighter.gameObject.activeSelf;
            
            // 장비 장착
            transform.parent.GetComponent<InvenGridManager>().placeholder.SetActive(false);
            transform.parent.GetComponent<InvenGridManager>().highlighter.SetActive(true);
            gridManager.RemoveSelectedButton();
        }
        else
        {
            // 장비 탈착
            transform.parent.GetComponent<InvenGridManager>().placeholder.SetActive(true);
            transform.parent.GetComponent<InvenGridManager>().highlighter.SetActive(false);
        }
    }
}
