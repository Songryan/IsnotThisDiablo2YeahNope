using System.Collections.Generic;

namespace BehaviorTree
{
    // NodeState ������: ����� ���� ���¸� ��Ÿ���ϴ�.
    // RUNNING: ��尡 ���� ������ ��Ÿ���ϴ�.
    // SUCCESS: ��尡 ���������� �Ϸ�Ǿ����� ��Ÿ���ϴ�.
    // FAILURE: ��尡 ���������� ��Ÿ���ϴ�.
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    // Node Ŭ����: �ൿ Ʈ���� �⺻ ������, ����, �θ�, �ڽ� ��� �� �����͸� �����մϴ�.
    public class Node
    {
        // ����� ���� ���¸� �����մϴ�.
        protected NodeState state;

        // �θ� ��带 �����մϴ�.
        public Node parent;

        // �ڽ� ������ �����մϴ�.
        protected List<Node> children = new List<Node>();

        // ����� ������ ���ؽ�Ʈ�� �����մϴ�.
        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        // �⺻ ������: �θ� ��带 null�� �ʱ�ȭ�մϴ�.
        public Node()
        {
            parent = null;
        }

        // �ڽ� ��带 �޾Ƶ��̴� ������: �ڽ� ��带 �θ� ��忡 �����մϴ�.
        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                _Attach(child); // �� �ڽ� ��带 �� ��忡 �����մϴ�.
            }
        }

        // �ڽ� ��带 �θ� ��忡 �����մϴ�.
        private void _Attach(Node node)
        {
            node.parent = this; // �ڽ� ����� �θ� ���� ���� �����մϴ�.
            children.Add(node); // �ڽ� ��带 ���� ����� �ڽ� ��Ͽ� �߰��մϴ�.
        }

        // Evaluate �޼���: ����� ���¸� ���մϴ�. �⺻������ ���� ���¸� ��ȯ�մϴ�.
        public virtual NodeState Evaluate() => NodeState.FAILURE;

        // SetData �޼���: ������ ���ؽ�Ʈ�� Ű-�� ���� �����մϴ�.
        public void SetData(string key, object value)
        {
            _dataContext[key] = value; // ������ Ű�� ���� �����մϴ�.
        }

        // GetData �޼���: ������ ���ؽ�Ʈ���� Ű�� �ش��ϴ� ���� ��ȯ�մϴ�.
        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value)) // ���� ����� ������ ���ؽ�Ʈ���� ���� ã���ϴ�.
                return value;

            Node node = parent;
            while (node != null) // �θ� ��忡�� ���� ã�� ���� ��ȸ�մϴ�.
            {
                value = node.GetData(key); // �θ� ����� ������ ���ؽ�Ʈ���� ���� ã���ϴ�.
                if (value != null) return value; // ���� ã���� ��ȯ�մϴ�.
                node = node.parent; // ���� �θ� ���� �̵��մϴ�.
            }
            return null; // ���� ã�� ���ϸ� null�� ��ȯ�մϴ�.
        }

        // ClearData �޼���: ������ ���ؽ�Ʈ���� Ű�� �ش��ϴ� �����͸� �����մϴ�.
        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key)) // ���� ����� ������ ���ؽ�Ʈ�� Ű�� �ִ��� Ȯ���մϴ�.
            {
                _dataContext.Remove(key); // Ű�� ������ �����͸� �����մϴ�.
                return true; // ���� ���� �� true�� ��ȯ�մϴ�.
            }

            Node node = parent;
            while (node != null) // �θ� ��忡�� �����͸� �����ϱ� ���� ��ȸ�մϴ�.
            {
                bool cleared = node.ClearData(key); // �θ� ����� ������ ���ؽ�Ʈ���� �����͸� �����մϴ�.
                if (cleared)
                    return true; // ���� ���� �� true�� ��ȯ�մϴ�.
                node = node.parent; // ���� �θ� ���� �̵��մϴ�.
            }
            return false; // ���� ���� �� false�� ��ȯ�մϴ�.
        }
    }
}
