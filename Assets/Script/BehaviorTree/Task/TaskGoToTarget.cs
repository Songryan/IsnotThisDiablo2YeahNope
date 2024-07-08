using BehaviorTree;
using UnityEngine;

// TaskGoToTarget Ŭ����: AI ĳ���Ͱ� ��ǥ �������� �̵��ϴ� �ൿ�� �����մϴ�.
public class TaskGoToTarget : Node
{
    // _transform: AI ĳ������ Transform ������Ʈ�� �����մϴ�.
    private Transform _transform;

    // TaskGoToTarget ������: Transform ������Ʈ�� �޾� �ʱ�ȭ�մϴ�.
    public TaskGoToTarget(Transform transform)
    {
        _transform = transform; // AI ĳ������ Transform�� �����մϴ�.
    }

    // Evaluate �޼���: ����� ���¸� ���ϰ� ��ǥ �������� �̵��մϴ�.
    public override NodeState Evaluate()
    {
        // ������ ���ؽ�Ʈ���� "target" �����͸� �����ɴϴ�.
        Transform target = (Transform)GetData("target");

        // ��ǥ �������� �Ÿ��� 0.01���� ū ��� �̵��� �����մϴ�.
        if (Vector3.Distance(_transform.position, target.position) > 0.01f)
        {
            // ��ǥ ������ Y ��ǥ�� 0���� �����մϴ�.
            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);

            // AI ĳ���͸� ��ǥ �������� �̵���ŵ�ϴ�.
            _transform.position = Vector3.MoveTowards(
                _transform.position, targetPosition, MonsterBT.Runspeed * Time.deltaTime);

            // AI ĳ���Ͱ� ��ǥ ������ �ٶ󺸵��� ȸ����ŵ�ϴ�.
            Vector3 lookAtPosition = new Vector3(target.position.x, _transform.position.y, target.position.z);
            _transform.LookAt(lookAtPosition);
        }

        // ����� ���¸� ���� ������ �����մϴ�.
        state = NodeState.RUNNING;
        return state; // ���� �� ���¸� ��ȯ�մϴ�.
    }
}
