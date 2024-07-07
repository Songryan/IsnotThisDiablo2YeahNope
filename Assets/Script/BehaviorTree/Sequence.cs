using System.Collections.Generic;

namespace BehaviorTree
{
    // Sequence Ŭ����: �ൿ Ʈ���� ���� ����, �ڽ� ������ ���������� ���մϴ�.
    // ��� �ڽ� ��尡 �����ؾ� ���� ���¸� ��ȯ�մϴ�.
    public class Sequence : Node
    {
        // �⺻ ������: �ڽ� ��尡 ���� Sequence ��带 �ʱ�ȭ�մϴ�.
        public Sequence() : base() { }

        // �ڽ� ��带 �޾Ƶ��̴� ������: �ڽ� ��带 ���� Sequence ��带 �ʱ�ȭ�մϴ�.
        public Sequence(List<Node> children) : base(children) { }

        // Evaluate �޼���: �ڽ� ������ ���������� ���Ͽ� Sequence ����� ���¸� �����մϴ�.
        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false; // �ڽ� ��� �� ���� ���� ��尡 �ִ��� ���θ� �����մϴ�.

            foreach (Node node in children) // ��� �ڽ� ��带 ���������� ���մϴ�.
            {
                switch (node.Evaluate()) // ���� �ڽ� ����� ���¸� ���մϴ�.
                {
                    case NodeState.FAILURE: // �ڽ� ��尡 ������ ���
                        state = NodeState.FAILURE; // Sequence ��� ���¸� ���з� �����մϴ�.
                        return state; // ���� ���¸� ��ȯ�ϰ� �򰡸� �����մϴ�.
                    case NodeState.SUCCESS: // �ڽ� ��尡 ������ ���
                        continue; // ���� �ڽ� ��带 ���մϴ�.
                    case NodeState.RUNNING: // �ڽ� ��尡 ���� ���� ���
                        anyChildIsRunning = true; // ���� ���� ��尡 ������ ����մϴ�.
                        continue; // ���� �ڽ� ��带 ���մϴ�.
                    default: // ����ġ ���� ������ ���
                        state = NodeState.SUCCESS; // �⺻������ ���� ���·� �����մϴ�.
                        return state; // ���� ���¸� ��ȯ�ϰ� �򰡸� �����մϴ�.
                }
            }

            // ��� �ڽ� ����� �򰡰� �Ϸ�� ��
            // ���� ���� �ڽ� ��尡 ������ ���� �� ���¸�, �׷��� ������ ���� ���¸� ��ȯ�մϴ�.
            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}
