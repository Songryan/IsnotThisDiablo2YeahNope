using System;
using System.Collections;
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
/*
    public void RefreshCharacterInfo(Action<string, string, int> callback)
    {
        StartCoroutine(WaitForCharacterStats(callback));
    }

    private IEnumerator WaitForCharacterStats(Action<string, string, int> callback)
    {
        bool isLoaded = false;

        // ������ �ε尡 �Ϸ�Ǿ��� �� ����� �ݹ�
        Action onComplete = () => { isLoaded = true; };

        // ĳ���� ���� �ε� ����
        JsonDataManager.Instance.LoadCharacterStats(onComplete);

        // ������ �ε尡 �Ϸ�� ������ ��ٸ�
        while (!isLoaded)
        {
            yield return null;
        }

        // ������ �ε尡 �Ϸ�� �� ����
        string userID = string.Empty;
        string userName = string.Empty;
        foreach (var value in JsonDataManager.Instance.CharStrProp)
        {
            userID = value.Key;
            userName = value.Value;
        }

        callback.Invoke(userID, userName, JsonDataManager.Instance.CharIntProp[$"{userID}_Level"]);
    }*/


}