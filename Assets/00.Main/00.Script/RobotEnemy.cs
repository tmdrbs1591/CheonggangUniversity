using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotEnemy : EnemyBase
{
    [SerializeField] protected Transform attackPos;
    [SerializeField] protected Vector2 attackBoxSize;
    private void Start()
    {
        base.Start();
    }
    private void Update()
    {
        base.Update();
    }
    protected override IEnumerator Cor_Attack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        dangerLineRenderer.enabled = true;

        dangerLine.SetActive(true);
        yield return new WaitForSeconds(1f);
        dangerLine.SetActive(false);

        Damage();
        // 쿨타임 시작
        currentCoolTime = attackCoolTime;

        // 공격 후 대기 시간
        yield return new WaitForSeconds(1f);

        isAttacking = false;
    }

    private void Damage()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackPos.position, attackBoxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            Debug.Log("공격!");
            if (collider != null)
            {
                if (collider.gameObject.CompareTag("Player"))
                {
                    collider.GetComponent<PlayerBase>().TakeDamage(1);

                }

            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, attackBoxSize);
    }
}
