using UnityEngine;
using BehaviorTree;

public class TaskPlayerAttack : Node
{
    private Animator _animator;

    private Transform _lastTarget;
    private MonsterManager _monsterManager;

    private float _attackTime = 1f;
    private float _attackCount = 0f;

    public TaskPlayerAttack(Transform transform)
    {
         _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if(target != _lastTarget)
        {
            _monsterManager = target.GetComponent<MonsterManager>();
            _lastTarget = target;
        }

        _attackCount += Time.deltaTime;
        if(_attackCount >= _attackTime )
        {
            bool isDead = _monsterManager.TakeHit();
            _animator.SetTrigger("Hitting");

            if (isDead) 
            {
                ClearData("target");
                //_animator.SetBool("Attacking",false);
                _animator.ResetTrigger("Hitting");
                _animator.SetBool("Walking",true);
                _animator.SetBool("Run",false);
            }
            else
            {
                _attackCount = 0f;
            }
            _animator.ResetTrigger("Hitting");
        }

        state = NodeState.RUNNING;
        return state;
    }
}
