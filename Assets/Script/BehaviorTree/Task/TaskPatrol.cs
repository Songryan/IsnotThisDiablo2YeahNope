using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;
using System.Net;

// TaskPatrol 클래스: AI 캐릭터가 정해진 웨이포인트를 따라 순찰하는 행동을 정의합니다.
public class TaskPatrol : Node
{
    // AI 캐릭터의 Transform 컴포넌트를 저장합니다.
    private Transform _transform;

    private Animator _animator;

    private NavMeshAgent navMesh;

    // 순찰할 웨이포인트 배열을 저장합니다.
    private Transform[] _waypoints;

    // 현재 목표로 하는 웨이포인트의 인덱스를 저장합니다.
    private int _currentWaypointIndex = 0;

    // 웨이포인트에서 대기할 시간을 설정합니다 (초 단위).
    private float _waitTime = 1f;
    private float _waitCounter = 0f;    // 대기 시간을 추적합니다.
    private bool _waiting = false;      // 대기 중인지 여부를 추적합니다.

    // TaskPatrol 생성자: Transform 컴포넌트와 웨이포인트 배열을 받아 초기화합니다.
    public TaskPatrol(Transform transform, Transform[] waypoints, NavMeshAgent navMesh)
    {
        _transform = transform;     // AI 캐릭터의 Transform을 설정합니다.
        _animator = transform.GetComponent<Animator>();
        _waypoints = waypoints;     // 웨이포인트 배열을 설정합니다.
        this.navMesh = navMesh;
    }

    // Evaluate 메서드: 노드의 상태를 평가하고 AI 캐릭터의 순찰 동작을 업데이트합니다.
    public override NodeState Evaluate()
    {
        if (_waiting) // 대기 중인 경우
        {
            _waitCounter += Time.deltaTime; // 대기 시간을 증가시킵니다.
            if (_waitCounter >= _waitTime) // 대기 시간이 끝난 경우
            {
                _waiting = false; // 대기를 종료합니다.
                _animator.SetBool("Walking", true);
            }
        }
        else // 대기 중이 아닌 경우
        {
            // 현재 웨이포인트를 가져옵니다.
            Transform wp = _waypoints[_currentWaypointIndex];

            // AI 캐릭터가 웨이포인트에 도착한 경우
            if (Vector3.Distance(_transform.position, wp.position) < 0.1f)
            {
                _transform.position = wp.position; // 정확한 위치로 설정합니다.
                _waitCounter = 0f; // 대기 시간 카운터를 초기화합니다.
                _waiting = true; // 대기 상태로 전환합니다.

                // 다음 웨이포인트 인덱스를 계산합니다.
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                _animator.SetBool("Walking", false);
            }
            else // 웨이포인트로 이동 중인 경우
            {
                // AI 캐릭터를 웨이포인트 방향으로 이동시킵니다.
                //_transform.position = Vector3.MoveTowards(_transform.position, wp.position, MonsterBT.speed * Time.deltaTime);
                navMesh.SetDestination(wp.position);
                navMesh.speed = MonsterBT.speed;

                // AI 캐릭터가 웨이포인트를 바라보도록 회전시킵니다.
                //_transform.LookAt(wp.position);
            }
        }

        // 노드의 상태를 실행 중으로 설정합니다.
        state = NodeState.RUNNING;
        return state; // 실행 중 상태를 반환합니다.
    }
}
