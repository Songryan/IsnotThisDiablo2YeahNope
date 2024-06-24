using UnityEngine;

public class MouseToTouch : MonoBehaviour
{
    void Update()
    {
        // ���콺 ��ư�� ��ġó�� ó��
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(0, Input.mousePosition, TouchPhase.Began);
        }
        if (Input.GetMouseButton(0))
        {
            HandleTouch(0, Input.mousePosition, TouchPhase.Moved);
        }
        if (Input.GetMouseButtonUp(0))
        {
            HandleTouch(0, Input.mousePosition, TouchPhase.Ended);
        }
    }

    void HandleTouch(int fingerId, Vector2 position, TouchPhase phase)
    {
        // ��ġ �̺�Ʈ ó��
        Touch touch = new Touch
        {
            fingerId = fingerId,
            position = position,
            phase = phase,
            deltaTime = Time.deltaTime,
            tapCount = 1,
            type = TouchType.Direct
        };

        // Unity�� Touch �ý��۰� ȣȯ�ǵ��� �մϴ�.
        // ���⼭ �ʿ��� ó�� ������ �߰��ϼ���.
        Debug.Log($"Finger ID: {touch.fingerId}, Position: {touch.position}, Phase: {touch.phase}");
    }
}
