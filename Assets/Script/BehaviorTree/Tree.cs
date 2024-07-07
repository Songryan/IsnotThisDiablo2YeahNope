using UnityEngine;

namespace BehaviorTree
{
    // Tree 클래스: Unity에서 Behavior Tree의 루트 노드를 설정하고 평가를 관리하는 기본 클래스입니다.
    public abstract class Tree : MonoBehaviour
    {
        // _root: 행동 트리의 루트 노드를 저장합니다.
        protected Node _root = null;

        // Start 메서드: MonoBehaviour의 기본 Start 메서드로, 행동 트리의 루트 노드를 설정합니다.
        protected void Start()
        {
            _root = SetupTree(); // SetupTree 메서드를 호출하여 루트 노드를 설정합니다.
        }

        // Update 메서드: MonoBehaviour의 기본 Update 메서드로, 매 프레임마다 호출되어 행동 트리를 평가합니다.
        private void Update()
        {
            if (_root != null) // 루트 노드가 null이 아닌지 확인합니다.
                _root.Evaluate(); // 루트 노드를 평가하여 AI의 행동을 결정합니다.
        }

        // SetupTree 메서드: 행동 트리를 설정하는 추상 메서드입니다.
        // 이 메서드는 파생 클래스에서 구현되어 트리 구조를 정의합니다.
        protected abstract Node SetupTree();
    }
}
