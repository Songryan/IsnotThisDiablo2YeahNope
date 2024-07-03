using System;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    private Action<int, int> _levelUpCallback;

    [SerializeField] private GameObject UIRootObj;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UIManager).ToString());
                    _instance = singletonObject.AddComponent<UIManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void EnableRootUI()
    {
        UIRootObj.SetActive(true);
    }
}