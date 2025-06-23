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
                if (GameManager.instance.playerCont.playerStat.currentHp + item.value <= GameManager.instance.playerCont.playerStat.maxHp)
                {
                    GameManager.instance.playerCont.playerStat.IncreaseHealth(item.value);
                    GameManager.instance.playerCont.playerStat.UpdateUI();
                    RemoveItem();
                }
                else
                {

                }
                    break;
            case Item.ItemType.ManaPotion:
                Debug.Log("마나먹방");
                RemoveItem();
                break;
        }
    }
}
