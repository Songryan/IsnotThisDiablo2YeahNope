using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Button _button;
    private RectTransform _textPosition;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _textPosition = GetComponent<Transform>().GetChild(0).GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_button.interactable)
        {
            _textPosition.Translate(new Vector3(-4, -4, 0));
            // Pressed 상태의 스프라이트는 Button 컴포넌트가 자동으로 변경
            // 버튼 클릭 소리 재생
            //AudioManager.instance.Play(SoundInfo.cursorButtonClick);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_button.interactable)
        {
            _textPosition.Translate(new Vector3(4, 4, 0));
            // Normal 상태의 스프라이트는 Button 컴포넌트가 자동으로 변경
        }
    }
}
