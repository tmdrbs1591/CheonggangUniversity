using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public override void TakeDamage(float amount)
    {
        if (isDying) return;

        hpSlider.gameObject.SetActive(true);
        hp -= amount;
        AudioManager.instance?.PlaySound(transform.position, "RobotHit", Random.Range(1f, 1.1f), 1f);


        var randomOffset = (Vector2)Random.insideUnitCircle * 1f;

        var damageText = ObjectPool.SpawnFromPool("DamageText", transform.position + (Vector3)randomOffset);
        damageText.GetComponent<TMPro.TMP_Text>().text = amount.ToString();


        if (playerTransform != null)
        {
            Vector2 knockbackDir = (transform.position - playerTransform.position).normalized;
            rb.AddForce(knockbackDir * 1f, ForceMode2D.Impulse);
        }

        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);
        hitCoroutine = StartCoroutine(Cor_HitMaterialChange());

        Debug.Log($"Enemy damaged! HP: {hp}");
        CameraShake.instance.ShakeCamera(5f, 0.15f);

        hpSlider.value = hp / maxHp;

        if (hp <= 0)
            StartCoroutine(Cor_Die());
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
                    collider.GetComponent<PlayerBase>().TakeDamage(10);

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
