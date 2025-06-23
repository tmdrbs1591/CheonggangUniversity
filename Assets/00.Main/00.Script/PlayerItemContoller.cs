using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemContoller : MonoBehaviour
{
    [SerializeField] public Transform boxPos;
    [SerializeField] public Vector2 BoxSize;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ItemPickUp();
        }
    }
    public void ItemPickUp()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(boxPos.position, BoxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            Debug.Log("АјАн!");
            if (collider != null)
            {
                if (collider.gameObject.CompareTag("Item"))
                {
                    collider.GetComponent<ItemPickup>().Pickup();
                }

            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boxPos.position, BoxSize);
    }
}
