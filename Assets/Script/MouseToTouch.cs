using UnityEngine;

public class MouseToTouch : MonoBehaviour
{
    private static MouseToTouch _instance;
    public static MouseToTouch Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("MouseToTouch");
                _instance = go.AddComponent<MouseToTouch>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public delegate void TouchEvent(int fingerId, Vector2 position, TouchPhase phase);
    public event TouchEvent OnTouchEvent;

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
        // 터치 이벤트를 델리게이트로 처리
        OnTouchEvent?.Invoke(fingerId, position, phase);
    }
}
