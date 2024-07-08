using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckEnemyInAttackRangeOfPlayer : Node
{
    //private static int _enemyLayerMask = LayerMask.GetMask("Player");

    private Transform _transform;
    private Animator _animator;

    private NavMeshAgent _navMeshAgent;

    public CheckEnemyInAttackRangeOfPlayer(Transform transform, NavMeshAgent _navMeshAgent)
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

        // Ÿ���� ��ȿ���� Ȯ��
        Transform target = (Transform)t;
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            ClearData("target");
            _animator.SetBool("Walking", true);
            state = NodeState.FAILURE;
            return state;
        }

        if (Vector3.Distance(_transform.position, target.position) <= PlayerBT.attackRange)
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

        state = NodeState.FAILURE;
        return state;
    }
}
