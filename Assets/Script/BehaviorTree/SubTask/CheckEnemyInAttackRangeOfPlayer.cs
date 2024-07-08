using BehaviorTree;
using UnityEngine;

public class CheckEnemyInAttackRangeOfPlayer : Node
{
    //private static int _enemyLayerMask = LayerMask.GetMask("Player");

    private Transform _transform;
    private Animator _animator;

    public CheckEnemyInAttackRangeOfPlayer(Transform transform)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");

        if (t == null)
        {
            state = NodeState.FAILURE;
            return state;
        }
        
        Transform target = (Transform)t;
        if(Vector3.Distance(_transform.position, target.position) <= PlayerBT.attackRange)
        {
            _animator.SetBool("Attacking", true);
            _animator.SetBool("Walking", false);
            _animator.SetBool("Run", false);

            state = NodeState.SUCCESS;
            return state;
        }

        _animator.SetBool("Attacking", false);

        state = NodeState.FAILURE;
        return state;
    }
}
