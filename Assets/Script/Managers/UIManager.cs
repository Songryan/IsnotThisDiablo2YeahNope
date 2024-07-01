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

        // 데이터 로드가 완료되었을 때 실행될 콜백
        Action onComplete = () => { isLoaded = true; };

        // 캐릭터 스탯 로드 시작
        JsonDataManager.Instance.LoadCharacterStats(onComplete);

        // 데이터 로드가 완료될 때까지 기다림
        while (!isLoaded)
        {
            yield return null;
        }

        // 데이터 로드가 완료된 후 실행
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