using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager instance;
    public List<Item> items = new List<Item>();

    public Transform itemContent;
    public GameObject inventoryItem;

    public Toggle enableRemove;

    public InventoryItemContoller[] inventoryItems;
    private void Awake()
    {
        instance = this;
    }
    public void Add(Item item)
    {
        items.Add(item);
        ListItems();
    }
    public void Remove(Item item)
    {
        items.Remove(item);
    }
    public void ListItems()
    {
        foreach (Transform item in itemContent)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in items)
        {
            GameObject obj = Instantiate(inventoryItem, itemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<TMP_Text>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();
            var controller = obj.GetComponent<InventoryItemContoller>();

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;

            controller.AddItem(item);

            if (enableRemove.isOn)
                removeButton.gameObject.SetActive(true);

            // removeButton 클릭 이벤트 등록 (한 번만)
            removeButton.onClick.RemoveAllListeners();
            removeButton.onClick.AddListener(() => controller.UseItem());
        }
        // SetInventoryItems();  // 이 줄은 제거
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
            inventoryItems[i].AddItem(items[i]);
        }
    }
}
