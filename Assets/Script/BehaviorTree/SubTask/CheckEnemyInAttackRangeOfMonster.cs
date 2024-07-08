using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;


public class CheckEnemyInAttackRangeOfMonster : Node
{
    //private static int _enemyLayerMask = LayerMask.GetMask("Player");

    private Transform _transform;
    private Animator _animator;

    private NavMeshAgent _navMeshAgent;

    public CheckEnemyInAttackRangeOfMonster(Transform transform, NavMeshAgent _navMeshAgent)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
        this._navMeshAgent = _navMeshAgent;
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
        if (Vector3.Distance(_transform.position, target.position) <= MonsterBT.attackRange)
        {
            //_animator.SetBool("Attacking", true);
            //_animator.SetTrigger("Hitting");
            _animator.SetBool("Walking", false);
            _animator.SetBool("Run", false);

            if (_navMeshAgent != null)
            {
                _navMeshAgent.isStopped = true; // NavMesh ������Ʈ �̵� ����
            }

            state = NodeState.SUCCESS;
            return state;
        }

        //_animator.SetBool("Attacking", false);
        //_animator.ResetTrigger("Hitting");

        if (_navMeshAgent != null)
        {
            _navMeshAgent.isStopped = false; // NavMesh ������Ʈ �̵� �簳
        }

        state = NodeState.FAILURE;
        return state;
    }
}
