using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private int _MaxLife;
    [SerializeField] private int _Life;

    [SerializeField] private int _Stamina;
    [SerializeField] private int _MaxStamina;

    [SerializeField] private int _Mana;
    [SerializeField] private int _MaxMana;

    [SerializeField] private int _Damage;
    [SerializeField] private int _AttackRating;
    [SerializeField] private int _Defense;
    [SerializeField] private int _ChanceToBlock;

    [SerializeField] private int _CharacterExp;
    [SerializeField] private int _CurrentExp;

    [SerializeField] private string current_UserID;

    [SerializeField] private Animator _animator;

    private void Awake()
    {
        current_UserID = JsonDataManager.Instance.current_UserID;

        //JsonDataManager.Instance.CharIntProp[$"{current_UserID}_Life"];
        
        _animator = transform.GetComponent<Animator>();

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
    }

    public bool TakeHit()
    {
        _animator.SetTrigger("Damaged");
        _Life -= 10;
        bool isDead = _Life <= 0;
        if (isDead) _Die();
        return isDead;
    }

    private void _Die()
    {
        Destroy(gameObject);
    }
}
