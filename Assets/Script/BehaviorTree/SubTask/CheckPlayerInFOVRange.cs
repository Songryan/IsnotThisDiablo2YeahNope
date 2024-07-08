using BehaviorTree;
using UnityEngine;

// CheckEnemyInFOVRange 클래스: AI 캐릭터가 시야(FOV) 범위 내에 적이 있는지 확인하는 노드입니다.
public class CheckPlayerInFOVRange : Node
{
    // _enemyLayerMask: 적이 있는 레이어를 나타내는 레이어 마스크입니다.
    //private static int _enemyLayerMask = 1 << 6;
    private static int _enemyLayerMask;

    // _transform: AI 캐릭터의 Transform 컴포넌트를 저장합니다.
    private Transform _transform;

    // _animator: AI 캐릭터의 Animator 컴포넌트를 저장합니다.
    private Animator _animator;

    private string TargetRole = string.Empty;

    // CheckEnemyInFOVRange 생성자: Transform 컴포넌트를 받아 초기화합니다.
    public CheckPlayerInFOVRange(Transform transform, string TargetRole)
    {
        this.TargetRole = TargetRole;
        _transform = transform; // AI 캐릭터의 Transform을 설정합니다.
        _animator = transform.GetComponent<Animator>(); // AI 캐릭터의 Animator를 설정합니다.
        _enemyLayerMask = LayerMask.GetMask(TargetRole);
    }

    // Evaluate 메서드: 노드의 상태를 평가하고 시야 범위 내의 적을 확인합니다.
    public override NodeState Evaluate()
    {
        // 데이터 컨텍스트에서 "target" 데이터를 가져옵니다.
        object t = GetData("target");

        // "target" 데이터가 없는 경우 시야 범위 내의 적을 찾습니다.
        if (t == null)
        {
            // 시야 범위 내의 적을 감지합니다.
            Collider[] colliders = Physics.OverlapSphere(
                _transform.position, MonsterBT.fovRange, _enemyLayerMask);

            // 감지된 적이 있는 경우
            if (colliders.Length > 0)
            {
                // "target" 데이터를 설정합니다.
                parent.parent.SetData("target", colliders[0].transform);
                _animator.SetBool("Run", true); // "Run" 애니메이션을 활성화합니다.
                state = NodeState.SUCCESS; // 노드 상태를 성공으로 설정합니다.
                return state; // 성공 상태를 반환합니다.
            }

            // 감지된 적이 없는 경우
            _animator.SetBool("Run", false); // "Run" 애니메이션을 비활성화합니다.
            state = NodeState.FAILURE; // 노드 상태를 실패로 설정합니다.
            return state; // 실패 상태를 반환합니다.
        }

        // "target" 데이터가 있는 경우
        state = NodeState.SUCCESS; // 노드 상태를 성공으로 설정합니다.
        return state; // 성공 상태를 반환합니다.
    }
}
