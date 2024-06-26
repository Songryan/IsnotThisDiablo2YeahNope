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
        // ��ġ �̺�Ʈ�� ��������Ʈ�� ó��
        OnTouchEvent?.Invoke(fingerId, position, phase);
    }
}
