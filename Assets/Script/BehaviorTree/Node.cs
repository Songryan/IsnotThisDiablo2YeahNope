using System.Collections.Generic;

namespace BehaviorTree
{
    // NodeState 열거형: 노드의 현재 상태를 나타냅니다.
    // RUNNING: 노드가 실행 중임을 나타냅니다.
    // SUCCESS: 노드가 성공적으로 완료되었음을 나타냅니다.
    // FAILURE: 노드가 실패했음을 나타냅니다.
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    // Node 클래스: 행동 트리의 기본 단위로, 상태, 부모, 자식 노드 및 데이터를 관리합니다.
    public class Node
    {
        // 노드의 현재 상태를 저장합니다.
        protected NodeState state;

        // 부모 노드를 참조합니다.
        public Node parent;

        // 자식 노드들을 저장합니다.
        protected List<Node> children = new List<Node>();

        // 노드의 데이터 컨텍스트를 저장합니다.
        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        // 기본 생성자: 부모 노드를 null로 초기화합니다.
        public Node()
        {
            parent = null;
        }

        // 자식 노드를 받아들이는 생성자: 자식 노드를 부모 노드에 연결합니다.
        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                _Attach(child); // 각 자식 노드를 이 노드에 연결합니다.
            }
        }

        // 자식 노드를 부모 노드에 연결합니다.
        private void _Attach(Node node)
        {
            node.parent = this; // 자식 노드의 부모를 현재 노드로 설정합니다.
            children.Add(node); // 자식 노드를 현재 노드의 자식 목록에 추가합니다.
        }

        // Evaluate 메서드: 노드의 상태를 평가합니다. 기본적으로 실패 상태를 반환합니다.
        public virtual NodeState Evaluate() => NodeState.FAILURE;

        // SetData 메서드: 데이터 컨텍스트에 키-값 쌍을 설정합니다.
        public void SetData(string key, object value)
        {
            _dataContext[key] = value; // 지정된 키에 값을 저장합니다.
        }

        // GetData 메서드: 데이터 컨텍스트에서 키에 해당하는 값을 반환합니다.
        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value)) // 현재 노드의 데이터 컨텍스트에서 값을 찾습니다.
                return value;

            Node node = parent;
            while (node != null) // 부모 노드에서 값을 찾기 위해 순회합니다.
            {
                value = node.GetData(key); // 부모 노드의 데이터 컨텍스트에서 값을 찾습니다.
                if (value != null) return value; // 값을 찾으면 반환합니다.
                node = node.parent; // 다음 부모 노드로 이동합니다.
            }
            return null; // 값을 찾지 못하면 null을 반환합니다.
        }

        // ClearData 메서드: 데이터 컨텍스트에서 키에 해당하는 데이터를 삭제합니다.
        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key)) // 현재 노드의 데이터 컨텍스트에 키가 있는지 확인합니다.
            {
                _dataContext.Remove(key); // 키가 있으면 데이터를 삭제합니다.
                return true; // 삭제 성공 시 true를 반환합니다.
            }

            Node node = parent;
            while (node != null) // 부모 노드에서 데이터를 삭제하기 위해 순회합니다.
            {
                bool cleared = node.ClearData(key); // 부모 노드의 데이터 컨텍스트에서 데이터를 삭제합니다.
                if (cleared)
                    return true; // 삭제 성공 시 true를 반환합니다.
                node = node.parent; // 다음 부모 노드로 이동합니다.
            }
            return false; // 삭제 실패 시 false를 반환합니다.
        }
    }
}
