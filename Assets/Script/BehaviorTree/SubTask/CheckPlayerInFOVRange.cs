using BehaviorTree;
using UnityEngine;

// CheckEnemyInFOVRange Ŭ����: AI ĳ���Ͱ� �þ�(FOV) ���� ���� ���� �ִ��� Ȯ���ϴ� ����Դϴ�.
public class CheckPlayerInFOVRange : Node
{
    // _enemyLayerMask: ���� �ִ� ���̾ ��Ÿ���� ���̾� ����ũ�Դϴ�.
    //private static int _enemyLayerMask = 1 << 6;
    private static int _enemyLayerMask;

    // _transform: AI ĳ������ Transform ������Ʈ�� �����մϴ�.
    private Transform _transform;

    // _animator: AI ĳ������ Animator ������Ʈ�� �����մϴ�.
    private Animator _animator;

    private string TargetRole = string.Empty;

    // CheckEnemyInFOVRange ������: Transform ������Ʈ�� �޾� �ʱ�ȭ�մϴ�.
    public CheckPlayerInFOVRange(Transform transform, string TargetRole)
    {
        this.TargetRole = TargetRole;
        _transform = transform; // AI ĳ������ Transform�� �����մϴ�.
        _animator = transform.GetComponent<Animator>(); // AI ĳ������ Animator�� �����մϴ�.
        _enemyLayerMask = LayerMask.GetMask(TargetRole);
    }

    // Evaluate �޼���: ����� ���¸� ���ϰ� �þ� ���� ���� ���� Ȯ���մϴ�.
    public override NodeState Evaluate()
    {
        // ������ ���ؽ�Ʈ���� "target" �����͸� �����ɴϴ�.
        object t = GetData("target");

        // "target" �����Ͱ� ���� ��� �þ� ���� ���� ���� ã���ϴ�.
        if (t == null)
        {
            // �þ� ���� ���� ���� �����մϴ�.
            Collider[] colliders = Physics.OverlapSphere(
                _transform.position, MonsterBT.fovRange, _enemyLayerMask);

            // ������ ���� �ִ� ���
            if (colliders.Length > 0)
            {
                // "target" �����͸� �����մϴ�.
                parent.parent.SetData("target", colliders[0].transform);
                _animator.SetBool("Run", true); // "Run" �ִϸ��̼��� Ȱ��ȭ�մϴ�.
                state = NodeState.SUCCESS; // ��� ���¸� �������� �����մϴ�.
                return state; // ���� ���¸� ��ȯ�մϴ�.
            }

            // ������ ���� ���� ���
            _animator.SetBool("Run", false); // "Run" �ִϸ��̼��� ��Ȱ��ȭ�մϴ�.
            state = NodeState.FAILURE; // ��� ���¸� ���з� �����մϴ�.
            return state; // ���� ���¸� ��ȯ�մϴ�.
        }

        // "target" �����Ͱ� �ִ� ���
        state = NodeState.SUCCESS; // ��� ���¸� �������� �����մϴ�.
        return state; // ���� ���¸� ��ȯ�մϴ�.
    }
}
