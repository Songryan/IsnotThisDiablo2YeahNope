using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] public int _healthpoints;
    [SerializeField] public int _maxHealthpoints;
    [SerializeField] private Animator _animator;
    [SerializeField] private int _Exp = 20;

    [SerializeField] private string current_UserID;

    private void Awake()
    {
        current_UserID = JsonDataManager.Instance.current_UserID;
        _animator = transform.GetComponent<Animator>();
        _healthpoints = 100;
        _maxHealthpoints = 100;
    }

    public bool TakeHit()
    {
        _animator.SetTrigger("Damaged");
        _healthpoints -= JsonDataManager.Instance.CharIntProp[$"{current_UserID}_Damage"];

        InGameUIManager.Instance.DecreaseMonsterHP(_healthpoints, _maxHealthpoints, name.Replace("(Clone)", ""));

        bool isDead = _healthpoints <= 0;
        if (isDead) _Die();
        return isDead;
    }

    private void _Die()
    {
        _animator.SetTrigger("Death");
        StartCoroutine(DelayDeath());
    }

    IEnumerator DelayDeath()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
