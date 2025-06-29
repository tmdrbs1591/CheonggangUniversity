using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class BigLaser : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(Random.Range(GameManager.instance.playerCont.playerStat.attackPower + 20, GameManager.instance.playerCont.playerStat.attackPower + 50));
            }
        }
    }
}
