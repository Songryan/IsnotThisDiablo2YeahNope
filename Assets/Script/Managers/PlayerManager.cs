using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    

    [SerializeField] private Animator _animator;

    private void Awake()
    {
        _animator = transform.GetComponent<Animator>();
    }

    public bool TakeHit()
    {
        _animator.SetTrigger("Damaged");
        //InGameUIManager.Instance._Life -= 10;
        InGameUIManager.Instance.DecreaseHP(10);
        bool isDead = InGameUIManager.Instance._Life <= 0;
        if (isDead) _Die();
        return isDead;
    }

    private void _Die()
    {
        Destroy(gameObject);
    }
}
