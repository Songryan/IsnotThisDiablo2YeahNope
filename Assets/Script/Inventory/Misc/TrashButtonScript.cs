using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrashButtonScript : MonoBehaviour {

    public Button button;
    public InvenGridManager invenManager;
    public ItemListManager listManager;

    private void Start()
    {
        button.onClick.AddListener(DestroyItem);
    }
    
    private void DestroyItem()
    {
        if (ItemScript.selectedItem != null)
        {
            string uniqueKey = ItemScript.selectedItem.transform.GetComponent<ItemScript>().item.UniqueKey;
            if(JsonDataManager.Instance.toJsonData.ContainsKey(uniqueKey))
                JsonDataManager.Instance.DeleteAndModifyJsonData(uniqueKey); // 리스트 아이템 삭제
            else
                JsonDataManager.Instance.DeleteItemFromJson(uniqueKey); // 인벤 아이템 삭제

            invenManager.RemoveSelectedButton();
            listManager.itemEquipPool.ReturnObject(ItemScript.selectedItem);
            ItemScript.ResetSelectedItem();
        }
    }
}
