using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Lightning : MonoBehaviour
{
    public ElectricChain electricChain;
    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();    
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("LightningBullet"))
        {
            electricChain.ActivateElectricChain();
            sprite.color = Color.white; 
            Destroy(collision.gameObject);
        }
    }
}
