﻿using System.Collections; // 컬렉션 인터페이스를 사용
using System.Collections.Generic; // 제네릭 컬렉션을 사용
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

    private void Start() // Unity에서 스크립트가 처음 실행될 때 호출되는 메서드
    {
        text.text = gridPos.x + "," + gridPos.y; // 슬롯의 텍스트를 그리드 위치로 설정
    }
}