using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemContoller : MonoBehaviour
{
    [SerializeField] public Transform boxPos;
    [SerializeField] public Vector2 BoxSize;
    [SerializeField] private GameObject itemInfoUI;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemInfoText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ItemPickUp();
        }
        ItemInfo();
    }
    public void ItemPickUp()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(boxPos.position, BoxSize, 0);

        foreach (Collider2D collider in collider2Ds)
        {
            if (collider != null && collider.CompareTag("Item"))
            {
                Debug.Log("아이템 주움: " + collider.name);
                collider.GetComponent<ItemPickup>().Pickup();
                break; // 한 개만 줍고 끝!
            }
        }
    }

    public void ItemInfo()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(boxPos.position, BoxSize, 0);
        bool foundItem = false;

        foreach (Collider2D collider in collider2Ds)
        {
            if (collider != null && collider.CompareTag("Item"))
            {
                foundItem = true;

                Debug.Log("공격!");

                itemInfoUI.SetActive(true);  // 다시 켜기

                var itemSc = collider.GetComponent<ItemPickup>();
                itemInfoUI.transform.position = collider.transform.position + new Vector3(0, 2, 0);
                itemImage.sprite = itemSc.item.icon;
                itemNameText.text = itemSc.item.itemName;
                itemInfoText.text = itemSc.item.itemInfo;

                break; // 하나만 처리하고 끝낼 경우
            }
        }

        if (!foundItem)
        {
            itemInfoUI.SetActive(false);
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boxPos.position, BoxSize);
    }
}
