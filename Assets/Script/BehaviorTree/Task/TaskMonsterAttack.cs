using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskMonsterAttack : Node
{
    private Animator _animator;

    private Transform _lastTarget;
    private PlayerManager _playerManager;
    private MonsterManager _monsterManager;

    private float _attackTime = 3f;
    private float _attackCount = 0f;

    public TaskMonsterAttack(Transform transform)
    {
        _animator = transform.GetComponent<Animator>();
        _monsterManager = transform.GetComponent<MonsterManager>();
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (target != _lastTarget)
        {
            _playerManager = target.GetComponent<PlayerManager>();
            _lastTarget = target;
        }

        _attackCount += Time.deltaTime;
        if (_attackCount >= _attackTime)
        {
            bool isDead = false;
            if (_monsterManager._healthpoints >= 0)
                isDead = _playerManager.TakeHit();

            _animator.SetTrigger("Hitting");

            if (isDead)
            {
                ClearData("target");
                //_animator.SetBool("Attacking", false);
                //_animator.ResetTrigger("Hitting");
                _animator.SetBool("Walking", true);
                _animator.SetBool("Run", false);
            }
            else
            {
                _attackCount = 0f;
            }
            //_animator.ResetTrigger("Hitting");
        }

        state = NodeState.RUNNING;
        return state;
    }
}
