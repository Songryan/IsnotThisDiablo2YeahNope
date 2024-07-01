using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    private Action<int, int> _levelUpCallback;

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
    public void RefreshCharacterInfo(Action<string, string, int> callback)
    {
        string UserID = string.Empty;
        string UserName = string.Empty;
        foreach (var value in JsonDataManager.Instance.CharStrProp)
        {
            UserID = value.Key;
            UserName = value.Value;
        }

        callback.Invoke(UserID, UserName, JsonDataManager.Instance.CharIntProp[$"{UserID}_Level"]);
    }
}