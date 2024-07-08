using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskMonsterAttack : Node
{
    private Animator _animator;

    private Transform _lastTarget;
    private PlayerManager _playerManager;

    private float _attackTime = 1f;
    private float _attackCount = 0f;

    public TaskMonsterAttack(Transform transform)
    {
        _animator = transform.GetComponent<Animator>();
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
            bool isDead = _playerManager.TakeHit();

            if (isDead)
            {
                ClearData("target");
                _animator.SetBool("Attacking", false);
                _animator.SetBool("Walking", true);
                _animator.SetBool("Run", false);
            }
            else
            {
                _attackCount = 0f;
            }
        }

        state = NodeState.RUNNING;
        return state;
    }
}
