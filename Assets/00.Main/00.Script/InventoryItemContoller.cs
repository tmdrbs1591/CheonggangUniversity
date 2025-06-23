using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemContoller : MonoBehaviour
{
    Item item;
    public Button RemoveButton;

    public void RemoveItem()
    {
        InventoryManager.instance.Remove(item);
        Destroy(gameObject);
    }
    public void AddItem(Item newItem)
    {
        item = newItem;  
    }

    public void UseItem()
    {
        switch(item.itemType)
        {
            case Item.ItemType.HealthPotion:
                GameManager.instance.playerCont.playerStat.IncreaseHealth(10);
                GameManager.instance.playerCont.playerStat.UpdateUI();
                Debug.Log("체력먹방");
                break;
            case Item.ItemType.ManaPotion:
                Debug.Log("마나먹방");
                break;
        }
        RemoveItem();
    }
}
