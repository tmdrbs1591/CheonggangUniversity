using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public string name;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Object"))
            {
                ObjectPool.SpawnFromPool("DroneBulletEffect", transform.position);
                ObjectPool.ReturnToPool(name, gameObject);
            }
            if (collision.gameObject.CompareTag("Player"))
            {
                ObjectPool.SpawnFromPool("DroneBulletEffect", transform.position);
                ObjectPool.ReturnToPool(name, gameObject);
                collision.GetComponent<PlayerBase>().TakeDamage(10);
            }
        }
    }
}
