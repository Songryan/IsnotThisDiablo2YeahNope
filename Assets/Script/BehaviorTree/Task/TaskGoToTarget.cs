using BehaviorTree;
using UnityEngine;

// TaskGoToTarget 클래스: AI 캐릭터가 목표 지점으로 이동하는 행동을 정의합니다.
public class TaskGoToTarget : Node
{
    // _transform: AI 캐릭터의 Transform 컴포넌트를 저장합니다.
    private Transform _transform;

    // TaskGoToTarget 생성자: Transform 컴포넌트를 받아 초기화합니다.
    public TaskGoToTarget(Transform transform)
    {
        _transform = transform; // AI 캐릭터의 Transform을 설정합니다.
    }

    // Evaluate 메서드: 노드의 상태를 평가하고 목표 지점으로 이동합니다.
    public override NodeState Evaluate()
    {
        // 데이터 컨텍스트에서 "target" 데이터를 가져옵니다.
        Transform target = (Transform)GetData("target");

        // 목표 지점과의 거리가 0.01보다 큰 경우 이동을 시작합니다.
        if (Vector3.Distance(_transform.position, target.position) > 0.01f)
        {
            // 목표 지점의 Y 좌표를 0으로 설정합니다.
            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);

            // AI 캐릭터를 목표 지점으로 이동시킵니다.
            _transform.position = Vector3.MoveTowards(
                _transform.position, targetPosition, MonsterBT.Runspeed * Time.deltaTime);

            // AI 캐릭터가 목표 지점을 바라보도록 회전시킵니다.
            Vector3 lookAtPosition = new Vector3(target.position.x, _transform.position.y, target.position.z);
            _transform.LookAt(lookAtPosition);
        }

        // 노드의 상태를 실행 중으로 설정합니다.
        state = NodeState.RUNNING;
        return state; // 실행 중 상태를 반환합니다.
    }
}
