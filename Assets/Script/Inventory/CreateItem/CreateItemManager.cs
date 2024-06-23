using UnityEngine; // Unity 엔진 기능을 사용
using UnityEngine.UI; // UI 요소를 사용

public class CreateItemManager : MonoBehaviour // CreateItemManager 클래스 정의, MonoBehaviour를 상속
{
    public ItemDatabase database; // 아이템 데이터베이스를 저장할 변수
    public ItemOverlayScript overlayScript; // 아이템 오버레이 스크립트를 저장할 변수
    public ObjectPoolScript itemEquipPool; // 아이템 장비 오브젝트 풀을 저장할 변수
    public ItemListManager listManager; // 아이템 리스트 매니저를 저장할 변수
    public SortAndFilterManager sortManager; // 정렬 및 필터 매니저를 저장할 변수

    public Dropdown nameDropdown; // 이름 드롭다운을 저장할 변수
    private int selectedID = 0; // 선택된 아이템 ID
    private bool isRandomType = false; // 랜덤 타입 여부

    public Slider lvlSlider; // 레벨 슬라이더를 저장할 변수
    public InputField lvlInput; // 레벨 입력 필드를 저장할 변수
    public Toggle randomLvlToggle; // 랜덤 레벨 토글을 저장할 변수
    private int selectedLvl = 1; // 선택된 레벨
    private bool isRandomLvl = false; // 랜덤 레벨 여부

    public Slider qualitySlider; // 품질 슬라이더를 저장할 변수
    public Toggle randomQualityToggle; // 랜덤 품질 토글을 저장할 변수
    public Text qualityText; // 품질 텍스트를 저장할 변수
    public GameObject QualityPanel; // 품질 패널을 저장할 변수
    private int selectedQuality = 0; // 선택된 품질
    private bool isRandomQuality = false; // 랜덤 품질 여부

    public Button createButton; // 생성 버튼을 저장할 변수
    public Button randomButton; // 랜덤 버튼을 저장할 변수
    public Toggle addToListToggle; // 리스트에 추가 토글을 저장할 변수
    public bool willAddToList = false; // 리스트에 추가 여부

    private ItemClass item = new ItemClass(); // 아이템 클래스 인스턴스를 저장할 변수

    private void Start() // Unity에서 스크립트가 처음 실행될 때 호출되는 메서드
    {
        nameDropdown.AddOptions(database.TypeNameList); // 데이터베이스에서 아이템 이름을 가져와 드롭다운에 추가
        lvlInput.text = "1"; // 레벨 입력 필드를 초기화
        ItemClass.SetItemValues(item, 0, 1, 0); // 아이템 값을 초기화
    }

    public void ButtonEnter() // 버튼에 마우스가 진입할 때 호출되는 메서드
    {
        ItemClass.SetItemValues(item, selectedID, selectedLvl, selectedQuality); // 아이템 값 설정
        overlayScript.UpdateOverlay2(item, !isRandomType, !isRandomLvl, !isRandomQuality); // 오버레이 업데이트
    }

    public void ButtonExit() // 버튼에서 마우스가 나갈 때 호출되는 메서드
    {
        ItemClass.SetItemValues(item, selectedID, selectedLvl, selectedQuality); // 아이템 값 설정
        overlayScript.UpdateOverlay(null); // 오버레이 초기화
    }

    #region type dropdown
    public void OnNameDropdownChange(int index) // 이름 드롭다운이 변경될 때 호출되는 메서드
    {
        selectedID = index; // 선택된 아이템 ID 설정
    }

    public void OnRandomTypeToggle(bool isToggled) // 랜덤 타입 토글이 변경될 때 호출되는 메서드
    {
        nameDropdown.interactable = !isToggled; // 드롭다운 활성화/비활성화
        isRandomType = isToggled; // 랜덤 타입 여부 설정
        if (!isToggled)
        {
            selectedID = nameDropdown.value; // 선택된 아이템 ID 설정
        }
    }
    #endregion

    #region lvl slider
    public void OnLvlSliderChange(float value) // 레벨 슬라이더가 변경될 때 호출되는 메서드
    {
        selectedLvl = (int)value; // 선택된 레벨 설정
        lvlInput.text = value.ToString(); // 레벨 입력 필드 업데이트
    }

    public void OnLvlInputChange(string value) // 레벨 입력 필드가 변경될 때 호출되는 메서드
    {
        if (value != "")
        {
            selectedLvl = int.Parse(value); // 선택된 레벨 설정
        }
        else
        {
            selectedLvl = 0; // 선택된 레벨 초기화
        }
        lvlSlider.value = selectedLvl; // 레벨 슬라이더 업데이트
    }

    public void OnRandomLvlToggleChange(bool isToggled) // 랜덤 레벨 토글이 변경될 때 호출되는 메서드
    {
        lvlSlider.interactable = !isToggled; // 레벨 슬라이더 활성화/비활성화
        lvlInput.interactable = !isToggled; // 레벨 입력 필드 활성화/비활성화
        isRandomLvl = isToggled; // 랜덤 레벨 여부 설정
        if (!isToggled)
        {
            selectedLvl = (int)lvlSlider.value; // 선택된 레벨 설정
        }
    }
    #endregion

    #region quality slider 
    // 품질 슬라이더
    public void OnQualitySliderChange(float value) // 품질 슬라이더가 변경될 때 호출되는 메서드
    {
        selectedQuality = (int)value; // 선택된 품질 설정
        string str = "";
        switch (selectedQuality)
        {
            case 0: str = "Broken"; break;
            case 1: str = "Normal"; break;
            case 2: str = "Magic"; break;
            case 3: str = "Rare"; break;
            default: break;
        }
        qualityText.text = str; // 품질 텍스트 업데이트
    }

    public void OnRandomQualityToggleChange(bool isToggled) // 랜덤 품질 토글이 변경될 때 호출되는 메서드
    {
        qualitySlider.interactable = !isToggled; // 품질 슬라이더 활성화/비활성화
        Image image = QualityPanel.GetComponent<Image>(); // 품질 패널의 이미지 컴포넌트를 가져옴
        image.color = isToggled ? new Color32(200, 200, 200, 128) : new Color32(255, 255, 255, 255); // 패널 색상 변경
        isRandomQuality = isToggled; // 랜덤 품질 여부 설정
        if (!isToggled)
        {
            selectedQuality = (int)qualitySlider.value; // 선택된 품질 설정
        }
    }
    #endregion

    #region button events
    public void CreateItem() // 생성 버튼을 클릭할 때 호출되는 메서드
    {
        if (ItemScript.selectedItem == null)
        {
            if (isRandomType) { selectedID = Random.Range(0, 18); } // 랜덤 타입 설정
            if (isRandomLvl) { selectedLvl = Random.Range(1, 101); } // 랜덤 레벨 설정
            if (isRandomQuality) { selectedQuality = Random.Range(0, 4); } // 랜덤 품질 설정
            ItemClass newItem = new ItemClass(item); // 새로운 아이템 생성
            ItemClass.SetItemValues(newItem, selectedID, selectedLvl, selectedQuality); // 아이템 값 설정
            SpawnOrAddItem(newItem); // 아이템 생성 또는 리스트에 추가
        }
    }

    public void RandomItem() // 랜덤 버튼을 클릭할 때 호출되는 메서드
    {
        if (ItemScript.selectedItem == null)
        {
            ItemClass newItem = new ItemClass(item); // 새로운 아이템 생성
            ItemClass.SetItemValues(newItem, Random.Range(0, 18), Random.Range(1, 101), Random.Range(0, 4)); // 아이템 값 설정
            SpawnOrAddItem(newItem); // 아이템 생성 또는 리스트에 추가
        }
    }

    private void SpawnOrAddItem(ItemClass passedItem) // 아이템을 생성하거나 리스트에 추가하는 메서드
    {
        if (willAddToList == false) // 마우스에 아이템 생성
        {
            GameObject itemObject = itemEquipPool.GetObject(); // 오브젝트 풀에서 아이템 가져옴
            itemObject.GetComponent<ItemScript>().SetItemObject(passedItem); // 아이템 설정
            ItemScript.SetSelectedItem(itemObject); // 선택된 아이템 설정
        }
        else // 리스트에 아이템 추가
        {
            sortManager.AddItemToList(passedItem); // 리스트에 아이템 추가
            Debug.Log("Item added to list");
        }
    }

    public void AddToListToggle(bool isToggled) // 리스트에 추가 토글이 변경될 때 호출되는 메서드
    {
        willAddToList = isToggled; // 리스트에 추가 여부 설정
    }
    #endregion
}
