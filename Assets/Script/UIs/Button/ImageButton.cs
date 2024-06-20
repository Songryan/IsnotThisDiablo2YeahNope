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
            // Pressed ������ ��������Ʈ�� Button ������Ʈ�� �ڵ����� ����
            // ��ư Ŭ�� �Ҹ� ���
            //AudioManager.instance.Play(SoundInfo.cursorButtonClick);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_button.interactable)
        {
            _textPosition.Translate(new Vector3(4, 4, 0));
            // Normal ������ ��������Ʈ�� Button ������Ʈ�� �ڵ����� ����
        }
    }
}
