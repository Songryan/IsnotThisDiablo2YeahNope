using UnityEngine;

public class MouseToTouch : MonoBehaviour
{
    void Update()
    {
        // 마우스 버튼을 터치처럼 처리
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
        // 터치 이벤트 처리
        Touch touch = new Touch
        {
            fingerId = fingerId,
            position = position,
            phase = phase,
            deltaTime = Time.deltaTime,
            tapCount = 1,
            type = TouchType.Direct
        };

        // Unity의 Touch 시스템과 호환되도록 합니다.
        // 여기서 필요한 처리 로직을 추가하세요.
        Debug.Log($"Finger ID: {touch.fingerId}, Position: {touch.position}, Phase: {touch.phase}");
    }
}
