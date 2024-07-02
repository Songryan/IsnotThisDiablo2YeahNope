using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private static ScenesManager _instance;
    public static ScenesManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScenesManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(ScenesManager).ToString());
                    _instance = singletonObject.AddComponent<ScenesManager>();
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

    public void StartBattle()
    {
        SceneManager.LoadScene("LoadingScene");
    }



}
