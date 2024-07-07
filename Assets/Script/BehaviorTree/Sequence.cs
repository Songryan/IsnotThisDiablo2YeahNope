using System.Collections.Generic;

namespace BehaviorTree
{
    // Sequence 클래스: 행동 트리의 제어 노드로, 자식 노드들을 순차적으로 평가합니다.
    // 모든 자식 노드가 성공해야 성공 상태를 반환합니다.
    public class Sequence : Node
    {
        // 기본 생성자: 자식 노드가 없는 Sequence 노드를 초기화합니다.
        public Sequence() : base() { }

        // 자식 노드를 받아들이는 생성자: 자식 노드를 가진 Sequence 노드를 초기화합니다.
        public Sequence(List<Node> children) : base(children) { }

        // Evaluate 메서드: 자식 노드들을 순차적으로 평가하여 Sequence 노드의 상태를 결정합니다.
        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false; // 자식 노드 중 실행 중인 노드가 있는지 여부를 저장합니다.

            foreach (Node node in children) // 모든 자식 노드를 순차적으로 평가합니다.
            {
                switch (node.Evaluate()) // 현재 자식 노드의 상태를 평가합니다.
                {
                    case NodeState.FAILURE: // 자식 노드가 실패한 경우
                        state = NodeState.FAILURE; // Sequence 노드 상태를 실패로 설정합니다.
                        return state; // 실패 상태를 반환하고 평가를 종료합니다.
                    case NodeState.SUCCESS: // 자식 노드가 성공한 경우
                        continue; // 다음 자식 노드를 평가합니다.
                    case NodeState.RUNNING: // 자식 노드가 실행 중인 경우
                        anyChildIsRunning = true; // 실행 중인 노드가 있음을 기록합니다.
                        continue; // 다음 자식 노드를 평가합니다.
                    default: // 예상치 못한 상태의 경우
                        state = NodeState.SUCCESS; // 기본적으로 성공 상태로 설정합니다.
                        return state; // 성공 상태를 반환하고 평가를 종료합니다.
                }
            }

            // 모든 자식 노드의 평가가 완료된 후
            // 실행 중인 자식 노드가 있으면 실행 중 상태를, 그렇지 않으면 성공 상태를 반환합니다.
            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}
