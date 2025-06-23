using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public float launchForce = 5f; // Æ¢´Â Èû ¼¼±â Á¶Àý

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            rb.AddForce(randomDir * launchForce, ForceMode2D.Impulse);
        }
    }

    public void Pickup()
    {
        InventoryManager.instance.Add(item);
        GameObject itemUI = ObjectPool.SpawnFromPool("ItemUI",GameManager.instance.itemUIPos.position);
        itemUI.transform.SetParent(GameManager.instance.itemUIPos, false);
        var itemName = itemUI.transform.Find("ItemName").GetComponent<TMP_Text>();
        var itemIcon = itemUI.transform.Find("ItemIcon").GetComponent<Image>();

        itemName.text = item.itemName + " X1";
        itemIcon.sprite = item.icon;
        Destroy(gameObject);
    }
}
