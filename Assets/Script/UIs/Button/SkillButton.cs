using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Button _button;
    private RectTransform _SkillImg;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _SkillImg = GetComponent<Transform>().GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_button.interactable)
        {
            _SkillImg.localScale = new Vector3(0.95f, 0.95f, 0);
            // Pressed ������ ��������Ʈ�� Button ������Ʈ�� �ڵ����� ����
            // ��ư Ŭ�� �Ҹ� ���
            //AudioManager.instance.Play(SoundInfo.cursorButtonClick);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_button.interactable)
        {
            _SkillImg.localScale = new Vector3(1f, 1f, 0);
            // Normal ������ ��������Ʈ�� Button ������Ʈ�� �ڵ����� ����
        }
    }
}
