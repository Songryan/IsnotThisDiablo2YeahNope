using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    private static InGameUIManager _instance;

    [Header("CharacterStat")]
    [SerializeField] private int _MaxLife;
    [SerializeField] public int _Life;

    [SerializeField] private int _Stamina;
    [SerializeField] private int _MaxStamina;

    [SerializeField] private int _Mana;
    [SerializeField] private int _MaxMana;

    [Header("CharacterStat2")]
    [SerializeField] private int _Damage;
    [SerializeField] private int _AttackRating;
    [SerializeField] private int _Defense;
    [SerializeField] private int _ChanceToBlock;

    [Header("CharacterStat3")]
    [SerializeField] private int _CharacterExp;
    [SerializeField] private int _CurrentExp;

    [Header("HP_UI")]
    [SerializeField] private Text hp_Text;
    [SerializeField] private Image hp_Image;

    [Header("MP_UI")]
    [SerializeField] private Text mp_Text;
    [SerializeField] private Image mp_Image;

    [Header("EXP_UI")]
    [SerializeField] private Image exp_Image;

    [Header("Stamina_UI")]
    [SerializeField] private Image stamina_Image;

    [Header("MonsterHP_UI")]
    [SerializeField] private GameObject EnemyBar;
    [SerializeField] private Text MonsterName_Text;
    [SerializeField] private Image MonsterHp_Image;

    [Header("AlertMsg")]
    [SerializeField] private GameObject alertMsgObj;
    [SerializeField] private Text Message;

    private string current_UserID;

    public static InGameUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InGameUIManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(InGameUIManager).ToString());
                    _instance = singletonObject.AddComponent<InGameUIManager>();
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

    public void RefreshUI()
    {
        #region 초기값 설정
        current_UserID = JsonDataManager.Instance.current_UserID;

        _MaxLife = JsonDataManager.Instance.CharIntProp[$"{current_UserID}_Life"];
        _MaxStamina = JsonDataManager.Instance.CharIntProp[$"{current_UserID}_Stamina"];
        _MaxMana = JsonDataManager.Instance.CharIntProp[$"{current_UserID}_Mana"];

        //_Damage = 100;
        _AttackRating = JsonDataManager.Instance.CharIntProp[$"{current_UserID}_AttackRating"];
        _Defense = JsonDataManager.Instance.CharIntProp[$"{current_UserID}_Defense"];
        _ChanceToBlock = JsonDataManager.Instance.CharIntProp[$"{current_UserID}_ChanceToBlock"];
        _CharacterExp = JsonDataManager.Instance.CharIntProp[$"{current_UserID}_CharacterExp"];
        _CurrentExp = JsonDataManager.Instance.CharIntProp[$"{current_UserID}_CurrentExp"];

        _Life = _MaxLife;
        _Mana = _MaxMana;
        _Stamina = _MaxStamina;
        #endregion

        #region UI 초기값 설정
        hp_Text.text = $"Life: {_MaxLife}/{_MaxLife}";
        hp_Image.fillAmount = _MaxLife / _MaxLife;

        mp_Text.text = $"Mana: {_MaxMana}/{_MaxMana}";
        mp_Image.fillAmount = _MaxMana / _MaxMana;

        exp_Image.fillAmount = (float)_CurrentExp / _CharacterExp;
        stamina_Image.fillAmount = (float)_MaxStamina / _MaxStamina;
        #endregion
    }

    public void DecreaseHP(int damage)
    {
        _Life -= damage;

        if(_Life <= 0)
            _Life = 0;

        hp_Text.text = $"Life: {_Life}/{_MaxLife}";
        hp_Image.fillAmount = (float)_Life / _MaxLife;
    }

    public void DecreaseMP(int damage)
    {
        _Mana -= damage;
        mp_Text.text = $"Mana: {_Mana}/{_MaxMana}";
        mp_Image.fillAmount = (float)_Mana / _MaxMana;
    }

    public void DecreaseMonsterHP(int currentHP, int maxHP, string name)
    {
        EnemyBar.SetActive(true);

        MonsterName_Text.text = name;

        if (currentHP <= 0)
            currentHP = 0;

        MonsterHp_Image.fillAmount = (float)currentHP / maxHP;

        StartCoroutine(OffEnemyBar());
    }

    IEnumerator OffEnemyBar()
    {
        yield return new WaitForSeconds(4f);
        EnemyBar.SetActive(false);
    }

    public void IncreaseEXP(int currentExp, int maxExp)
    {
        exp_Image.fillAmount = (float)currentExp / maxExp;
    }

    public void AlertGetItemMsg(string itemName, int gradeNum)
    {
        string grade = string.Empty;
        string color = string.Empty;

        switch (gradeNum)
        {
            case 0:
                grade = "Broken";
                color = "#808080"; // 회색
                break;
            case 1:
                grade = "Normal";
                color = "#FFFFFF"; // 흰색
                break;
            case 2:
                grade = "Magic";
                color = "#0000FF"; // 파란색
                break;
            case 3:
                grade = "Rare";
                color = "#FFFF00"; // 노란색
                break;
        }

        Message.text = $"You have acquired (<color={color}>[{grade}]</color>{itemName}).\nIt will automatically be saved\nto your inventory.";
        alertMsgObj.SetActive(true);

        StartCoroutine(OffAlertGetItemMsg());
    }

    IEnumerator OffAlertGetItemMsg()
    {
        yield return new WaitForSeconds(5f);
        alertMsgObj.SetActive(false);
    }
}
