using System.Collections.Generic;
using System.Net;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskSearchEndPoint : Node
{
    // AI ĳ������ Transform ������Ʈ�� �����մϴ�.
    private Transform _transform;

    private Animator _animator;

    private NavMeshAgent navMesh;

    public Transform EndPoint;

    // ������ ��������Ʈ �迭�� �����մϴ�.
    //private Transform[] _waypoints;

    // ���� ��ǥ�� �ϴ� ��������Ʈ�� �ε����� �����մϴ�.
    //private int _currentWaypointIndex = 0;

    // ��������Ʈ���� ����� �ð��� �����մϴ� (�� ����).
    //private float _waitTime = 1f;
    //private float _waitCounter = 0f;    // ��� �ð��� �����մϴ�.
    //private bool _waiting = false;      // ��� ������ ���θ� �����մϴ�.

    // TaskPatrol ������: Transform ������Ʈ�� ��������Ʈ �迭�� �޾� �ʱ�ȭ�մϴ�.
    public TaskSearchEndPoint(Transform transform, NavMeshAgent navMesh)
    {
        _transform = transform;     // AI ĳ������ Transform�� �����մϴ�.
        _animator = transform.GetComponent<Animator>();
        this.navMesh = navMesh;
        EndPoint = GameObject.FindWithTag("EndPoint").transform; // �±׸� �̿��Ͽ� �÷��̾��� Transform�� ã���ϴ�.
        //_waypoints = waypoints;     // ��������Ʈ �迭�� �����մϴ�.
    }

    // Evaluate �޼���: ����� ���¸� ���ϰ� AI ĳ������ ���� ������ ������Ʈ�մϴ�.
    public override NodeState Evaluate()
    {
        /*
        //if (_waiting) // ��� ���� ���
        //{
        //    _waitCounter += Time.deltaTime; // ��� �ð��� ������ŵ�ϴ�.
        //    if (_waitCounter >= _waitTime) // ��� �ð��� ���� ���
        //    {
        //        _waiting = false; // ��⸦ �����մϴ�.
        //        _animator.SetBool("Walking", true);
        //    }
        //}
        //else // ��� ���� �ƴ� ���
        //{
        //    // ���� ��������Ʈ�� �����ɴϴ�.
        //    Transform wp = _waypoints[_currentWaypointIndex];
        //
        //    // AI ĳ���Ͱ� ��������Ʈ�� ������ ���
        //    if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
        //    {
        //        _transform.position = wp.position; // ��Ȯ�� ��ġ�� �����մϴ�.
        //        _waitCounter = 0f; // ��� �ð� ī���͸� �ʱ�ȭ�մϴ�.
        //        _waiting = true; // ��� ���·� ��ȯ�մϴ�.
        //
        //        // ���� ��������Ʈ �ε����� ����մϴ�.
        //        _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
        //        _animator.SetBool("Walking", false);
        //    }
        //    else // ��������Ʈ�� �̵� ���� ���
        //    {
        //        // AI ĳ���͸� ��������Ʈ �������� �̵���ŵ�ϴ�.
        //        _transform.position = Vector3.MoveTowards(_transform.position, wp.position, PlayerBT.speed * Time.deltaTime);
        //
        //        // AI ĳ���Ͱ� ��������Ʈ�� �ٶ󺸵��� ȸ����ŵ�ϴ�.
        //        _transform.LookAt(wp.position);
        //    }
        //}
        */

        if (navMesh != null)
        {
            navMesh.isStopped = false; // NavMesh ������Ʈ �̵� �簳
        }

        if (EndPoint != null)
        {
            navMesh.SetDestination(EndPoint.position);
            navMesh.speed = PlayerBT.speed;
        }
        else
        {
            EndPoint = GameObject.FindWithTag("EndPoint").transform; // �±׸� �̿��Ͽ� �÷��̾��� Transform�� ã���ϴ�.
        }

        // ����� ���¸� ���� ������ �����մϴ�.
        state = NodeState.RUNNING;
        return state; // ���� �� ���¸� ��ȯ�մϴ�.
    }
}
