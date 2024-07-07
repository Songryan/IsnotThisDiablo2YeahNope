using System.Collections.Generic;

namespace BehaviorTree
{
    // Selector Ŭ����: �ൿ Ʈ���� ���� ����, �ڽ� ������ ���������� ���մϴ�.
    // �ڽ� ��� �� �ϳ��� �����ϸ� ���� ���¸� ��ȯ�մϴ�.
    public class Selector : Node
    {
        // �⺻ ������: �ڽ� ��尡 ���� Selector ��带 �ʱ�ȭ�մϴ�.
        public Selector() : base() { }

        // �ڽ� ��带 �޾Ƶ��̴� ������: �ڽ� ��带 ���� Selector ��带 �ʱ�ȭ�մϴ�.
        public Selector(List<Node> children) : base(children) { }

        // Evaluate �޼���: �ڽ� ������ ���������� ���Ͽ� Selector ����� ���¸� �����մϴ�.
        public override NodeState Evaluate()
        {
            foreach (Node node in children) // ��� �ڽ� ��带 ���������� ���մϴ�.
            {
                switch (node.Evaluate()) // ���� �ڽ� ����� ���¸� ���մϴ�.
                {
                    case NodeState.FAILURE: // �ڽ� ��尡 ������ ���
                        continue; // ���� �ڽ� ��带 ���մϴ�.
                    case NodeState.SUCCESS: // �ڽ� ��尡 ������ ���
                        state = NodeState.SUCCESS; // Selector ��� ���¸� �������� �����մϴ�.
                        return state; // ���� ���¸� ��ȯ�ϰ� �򰡸� �����մϴ�.
                    case NodeState.RUNNING: // �ڽ� ��尡 ���� ���� ���
                        state = NodeState.RUNNING; // Selector ��� ���¸� ���� ������ �����մϴ�.
                        return state; // ���� �� ���¸� ��ȯ�ϰ� �򰡸� �����մϴ�.
                    default: // ����ġ ���� ������ ���
                        continue; // ���� �ڽ� ��带 ���մϴ�.
                }
            }
            state = NodeState.FAILURE; // ��� �ڽ� ��尡 ������ ���
            return state; // ���� ���¸� ��ȯ�մϴ�.
        }
    }
}
