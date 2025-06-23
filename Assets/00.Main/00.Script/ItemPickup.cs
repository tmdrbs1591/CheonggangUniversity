using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Destroy(gameObject);
    }
}
