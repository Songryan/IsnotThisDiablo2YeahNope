using System; // �⺻ �ý��� ���̺귯���� ���
using System.Collections; // �÷��� �������̽��� ���
using System.Collections.Generic; // ���׸� �÷����� ���
using UnityEngine; // Unity ���� ����� ���
using UnityEngine.EventSystems; // �̺�Ʈ �ý����� ���

public class EquipSlotSectorScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler // SlotSectorScript Ŭ���� ����, MonoBehaviour�� �������̽��� ���
{
    public enum SlotType
    {
        Weapon,
        Shield,
        Armor,
        Glove,
        Boot,
        Helmet,
        Amulet,
        Ring,
        Belt,
        All
    }

    public SlotType slotType;

    public GameObject slotParent; // ������ �θ� ���� ������Ʈ�� ������ ����
    public int QuadNum; // ��и� ��ȣ�� ������ ����
    public static IntVector2 posOffset; // ��ġ �������� ������ ���� ����
    public static EquipSlotSectorScript equipSlotSectorScript; // ���� ���� ��ũ��Ʈ�� ������ ���� ����
    public static ItemOverlayScript overlayScript; // ������ �������� ��ũ��Ʈ�� ������ ���� ����
    private InvenGridManager invenGridManager; // �κ��丮 �׸��� �Ŵ����� ������ ����
    private SlotScript parentSlotScript; // �θ� ���� ��ũ��Ʈ�� ������ ����

    private void Start() // Unity���� ��ũ��Ʈ�� ó�� ����� �� ȣ��Ǵ� �޼���
    {
        invenGridManager = this.gameObject.transform.parent.parent.GetComponent<InvenGridManager>(); // �θ��� �θ� ������Ʈ���� InvenGridManager ������Ʈ�� ������
        parentSlotScript = slotParent.GetComponent<SlotScript>(); // �θ� ���� ������Ʈ���� SlotScript ������Ʈ�� ������
    }

    public void OnPointerEnter(PointerEventData eventData) // ���콺�� ���Կ� ������ �� ȣ��Ǵ� �޼���
    {
        equipSlotSectorScript = this; // ���� ���� ��ũ��Ʈ�� ����
        invenGridManager.highlightedSlot = slotParent; // ������ ������ �θ� �������� ����
        PosOffset(); // ��ġ ������ ����
        if (ItemScript.selectedItem != null) // ���õ� �������� ���� ���
        {
            invenGridManager.RefrechColor(true); // ���� ����
        }
        if (parentSlotScript.storedItemObject != null && ItemScript.selectedItem == null) // �θ� ���Կ� �������� ����Ǿ� �ְ� ���õ� �������� ���� ���
        {
            invenGridManager.ColorChangeLoop(SlotColorHighlights.Blue, parentSlotScript.storedItemSize, parentSlotScript.storedItemStartPos); // ���� ���� ����
        }
        if (parentSlotScript.storedItemObject != null) // �θ� ���Կ� �������� ����Ǿ� ���� ���
        {
            //overlayScript.UpdateOverlay(parentSlotScript.storedItemClass); // �������̸� ������ ������ ������Ʈ
        }
    }

    public void PosOffset() // ��ġ �������� �����ϴ� �޼���
    {
        if (ItemScript.selectedItemSize.x != 0 && ItemScript.selectedItemSize.x % 2 == 0) // ���õ� �������� ���� ũ�Ⱑ ¦���� ���
        {
            switch (QuadNum) // ��и� ��ȣ�� ���� ������ ����
            {
                case 1:
                    posOffset.x = 0; break;
                case 2:
                    posOffset.x = -1; break;
                case 3:
                    posOffset.x = 0; break;
                case 4:
                    posOffset.x = -1; break;
                default: break;
            }
        }
        if (ItemScript.selectedItemSize.y != 0 && ItemScript.selectedItemSize.y % 2 == 0) // ���õ� �������� ���� ũ�Ⱑ ¦���� ���
        {
            switch (QuadNum) // ��и� ��ȣ�� ���� ������ ����
            {
                case 1:
                    posOffset.y = -1; break;
                case 2:
                    posOffset.y = -1; break;
                case 3:
                    posOffset.y = 0; break;
                case 4:
                    posOffset.y = 0; break;
                default: break;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) // ���콺�� ���Կ��� ���� �� ȣ��Ǵ� �޼���
    {
        equipSlotSectorScript = null; // ���� ���� ��ũ��Ʈ�� �ʱ�ȭ
        invenGridManager.highlightedSlot = null; // ������ ������ �ʱ�ȭ
        //overlayScript.UpdateOverlay(null); // �������̸� �ʱ�ȭ
        if (ItemScript.selectedItem != null) // ���õ� �������� ���� ���
        {
            invenGridManager.RefrechColor(false); // ���� ����
        }
        posOffset = IntVector2.Zero; // ��ġ ������ �ʱ�ȭ
        if (parentSlotScript.storedItemObject != null && ItemScript.selectedItem == null) // �θ� ���Կ� �������� ����Ǿ� �ְ� ���õ� �������� ���� ���
        {
            invenGridManager.ColorChangeLoop(SlotColorHighlights.Blue2, parentSlotScript.storedItemSize, parentSlotScript.storedItemStartPos); // ���� ���� ����
        }
    }
}
