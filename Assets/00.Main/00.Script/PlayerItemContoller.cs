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
                Debug.Log("������ �ֿ�: " + collider.name);
                collider.GetComponent<ItemPickup>().Pickup();
                break; // �� ���� �ݰ� ��!
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

                Debug.Log("����!");

                itemInfoUI.SetActive(true);  // �ٽ� �ѱ�

                var itemSc = collider.GetComponent<ItemPickup>();
                itemInfoUI.transform.position = collider.transform.position + new Vector3(0, 2, 0);
                itemImage.sprite = itemSc.item.icon;
                itemNameText.text = itemSc.item.itemName;
                itemInfoText.text = itemSc.item.itemInfo;

                break; // �ϳ��� ó���ϰ� ���� ���
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
