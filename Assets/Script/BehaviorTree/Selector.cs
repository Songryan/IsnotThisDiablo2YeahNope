using System.Collections.Generic;

namespace BehaviorTree
{
    // Selector 클래스: 행동 트리의 제어 노드로, 자식 노드들을 순차적으로 평가합니다.
    // 자식 노드 중 하나라도 성공하면 성공 상태를 반환합니다.
    public class Selector : Node
    {
        // 기본 생성자: 자식 노드가 없는 Selector 노드를 초기화합니다.
        public Selector() : base() { }

        // 자식 노드를 받아들이는 생성자: 자식 노드를 가진 Selector 노드를 초기화합니다.
        public Selector(List<Node> children) : base(children) { }

        // Evaluate 메서드: 자식 노드들을 순차적으로 평가하여 Selector 노드의 상태를 결정합니다.
        public override NodeState Evaluate()
        {
            foreach (Node node in children) // 모든 자식 노드를 순차적으로 평가합니다.
            {
                switch (node.Evaluate()) // 현재 자식 노드의 상태를 평가합니다.
                {
                    case NodeState.FAILURE: // 자식 노드가 실패한 경우
                        continue; // 다음 자식 노드를 평가합니다.
                    case NodeState.SUCCESS: // 자식 노드가 성공한 경우
                        state = NodeState.SUCCESS; // Selector 노드 상태를 성공으로 설정합니다.
                        return state; // 성공 상태를 반환하고 평가를 종료합니다.
                    case NodeState.RUNNING: // 자식 노드가 실행 중인 경우
                        state = NodeState.RUNNING; // Selector 노드 상태를 실행 중으로 설정합니다.
                        return state; // 실행 중 상태를 반환하고 평가를 종료합니다.
                    default: // 예상치 못한 상태의 경우
                        continue; // 다음 자식 노드를 평가합니다.
                }
            }
            state = NodeState.FAILURE; // 모든 자식 노드가 실패한 경우
            return state; // 실패 상태를 반환합니다.
        }
    }
}
