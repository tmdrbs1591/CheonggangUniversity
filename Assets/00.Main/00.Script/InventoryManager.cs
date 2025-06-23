using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryItemData
{
    public Item item;
    public int quantity;

    public InventoryItemData(Item item, int quantity = 1)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager instance;
    public List<InventoryItemData> items = new List<InventoryItemData>();


    public Transform itemContent;
    public GameObject inventoryItem;

    public Toggle enableRemove;

    public InventoryItemContoller[] inventoryItems;

    public GameObject[] itemPrefabs;
 
    private void Awake()
    {
        instance = this;
    }
    public void Add(Item newItem)
    {
        var existingItem = items.Find(i => i.item == newItem);
        if (existingItem != null)
        {
            existingItem.quantity++;
        }
        else
        {
            items.Add(new InventoryItemData(newItem));
        }

        ListItems();
    }

    public void Remove(Item targetItem)
    {
        var existingItem = items.Find(i => i.item == targetItem);
        if (existingItem != null)
        {
            existingItem.quantity--;
            if (existingItem.quantity <= 0)
            {
                items.Remove(existingItem);
            }
            ListItems();
        }
    }

    public void ListItems()
    {
        foreach (Transform item in itemContent)
        {
            Destroy(item.gameObject);
        }
        foreach (var data in items)
        {
            GameObject obj = Instantiate(inventoryItem, itemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<TMP_Text>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var itemQuantity = obj.transform.Find("ItemQuantity").GetComponent<TMP_Text>(); // 새로 추가된 UI
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();
            var controller = obj.GetComponent<InventoryItemContoller>();

            itemName.text = data.item.itemName;
            itemIcon.sprite = data.item.icon;
            itemQuantity.text = $"x{data.quantity}";  // 수량 표시

            controller.AddItem(data.item);

            if (enableRemove.isOn)
                removeButton.gameObject.SetActive(true);

            removeButton.onClick.RemoveAllListeners();
            removeButton.onClick.AddListener(() => controller.UseItem());
        }

    }

    public void EnableItemRemove()
    {
        if (enableRemove.isOn)
        {
            foreach(Transform item in itemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform item in itemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(false );
            }
        }
    }

    public void SetInventoryItems()
    {
        inventoryItems = itemContent.GetComponentsInChildren<InventoryItemContoller>();

        for(int i = 0; i < items.Count; i++)
        {
            inventoryItems[i].AddItem(items[i].item);
        }
    }
}
