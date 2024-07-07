using UnityEngine;

namespace BehaviorTree
{
    // Tree Ŭ����: Unity���� Behavior Tree�� ��Ʈ ��带 �����ϰ� �򰡸� �����ϴ� �⺻ Ŭ�����Դϴ�.
    public abstract class Tree : MonoBehaviour
    {
        // _root: �ൿ Ʈ���� ��Ʈ ��带 �����մϴ�.
        protected Node _root = null;

        // Start �޼���: MonoBehaviour�� �⺻ Start �޼����, �ൿ Ʈ���� ��Ʈ ��带 �����մϴ�.
        protected void Start()
        {
            _root = SetupTree(); // SetupTree �޼��带 ȣ���Ͽ� ��Ʈ ��带 �����մϴ�.
        }

        // Update �޼���: MonoBehaviour�� �⺻ Update �޼����, �� �����Ӹ��� ȣ��Ǿ� �ൿ Ʈ���� ���մϴ�.
        private void Update()
        {
            if (_root != null) // ��Ʈ ��尡 null�� �ƴ��� Ȯ���մϴ�.
                _root.Evaluate(); // ��Ʈ ��带 ���Ͽ� AI�� �ൿ�� �����մϴ�.
        }

        // SetupTree �޼���: �ൿ Ʈ���� �����ϴ� �߻� �޼����Դϴ�.
        // �� �޼���� �Ļ� Ŭ�������� �����Ǿ� Ʈ�� ������ �����մϴ�.
        protected abstract Node SetupTree();
    }
}
