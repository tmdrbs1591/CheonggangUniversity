using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RiderBoss : EnemyBase
{
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private Slider baseHpSlider;
    [SerializeField] private GameObject busterEffect;
    [SerializeField] private GameObject dashAttackDangerArea;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashCheckDistance = 1f;
    [SerializeField] private int circleBulletCount = 12;
    [SerializeField] private float circleBulletInterval = 3f;
    [SerializeField] private float dashAttackInterval = 5f;
    [SerializeField] private GameObject skillItem;
    [SerializeField] private GameObject door;

    private float targetValue = 1f;
    private bool isDashing = false;
    public bool isBattle = false;
    private Vector2 dashDirection;

    private float circleBulletTimer = 0f;
    private float dashAttackTimer = 0f;


    private void Start()
    {
        baseHpSlider.gameObject.SetActive(false);
        SetHP();
        base.Start();
    }

    private void Update()
    {
        if (isDying || playerTransform == null) return;

        if (TimeLineManager.instance.isCutScene || !isBattle) return;
            base.Update();

        baseHpSlider.value = Mathf.Lerp(baseHpSlider.value, targetValue, Time.deltaTime * 4f);
        hpSlider.value = Mathf.Lerp(hpSlider.value, targetValue, Time.deltaTime * 8f);

        // 대시 공격 타이머
        dashAttackTimer += Time.deltaTime;
        if (dashAttackTimer >= dashAttackInterval)
        {
            dashAttackTimer = 0f;
            DashAttack();
        }

        // 원형 총알 발사 타이머
        circleBulletTimer += Time.deltaTime;
        if (circleBulletTimer >= circleBulletInterval)
        {
            circleBulletTimer = 0f;
            CircleBulletFire();
        }

        if (isDashing)
        {
            transform.Translate(dashDirection * dashSpeed * Time.deltaTime);
        }
    }

    private void DashAttack()
    {
        StartCoroutine(Cor_DashAttack());
    }

    IEnumerator Cor_DashAttack()
    {
        dashAttackDangerArea.SetActive(true);
        yield return new WaitForSeconds(1f);
        dashAttackDangerArea.SetActive(false);

        float horizontal = playerTransform.position.x - transform.position.x;
        if (horizontal > 0)
            dashDirection = Vector2.right;
        else
            dashDirection = Vector2.left;

        isDashing = true;
    }

    private void CircleBulletFire()
    {
        for (int i = 0; i < circleBulletCount; i++)
        {
            float angle = 360f / circleBulletCount * i;
            float radian = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;

            GameObject bulletObj = ObjectPool.SpawnFromPool("DroneBullet", transform.position);

            if (bulletObj != null)
            {
                Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    bulletRb.velocity = dir * bulletSpeed;
                }

                bulletObj.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    protected override IEnumerator Cor_Attack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        dangerLineRenderer.enabled = true;

        float elapsedTime = 0f;
        while (elapsedTime < lineDuration)
        {
            if (playerTransform == null) yield break;

            Vector2 startPoint = transform.position;
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            Vector2 endPoint = startPoint + direction * 20f;

            dangerLineRenderer.SetPosition(0, startPoint);
            dangerLineRenderer.SetPosition(1, endPoint);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dangerLineRenderer.enabled = false;
        Debug.Log("공격!");

        BulletFire();

        currentCoolTime = attackCoolTime;

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    void BulletFire()
    {
        if (playerTransform == null) return;

        GameObject bulletObj = ObjectPool.SpawnFromPool("DroneBullet", transform.position);
        if (bulletObj != null)
        {
            Vector2 shootDir = (playerTransform.position - transform.position).normalized;

            Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = shootDir * bulletSpeed;
            }

            float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
            bulletObj.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void SetHP()
    {
        targetValue = hp / maxHp;
    }

    public override void TakeDamage(float amount)
    {
        if (isDying) return;

        hp -= amount;
        AudioManager.instance?.PlaySound(transform.position, "Hit", Random.Range(1f, 1.1f), 1f);

        var randomOffset = (Vector2)Random.insideUnitCircle * 1.5f;
        var damageText = ObjectPool.SpawnFromPool("DamageText", transform.position + (Vector3)randomOffset);
        damageText.GetComponent<TMPro.TMP_Text>().text = amount.ToString();

        Vector2 knockbackDir = (transform.position - GameManager.instance.playerCont.transform.position).normalized;
        rb.AddForce(knockbackDir * 0.2f, ForceMode2D.Impulse);

        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }
        hitCoroutine = StartCoroutine(Cor_HitMaterialChange());

        Debug.Log($"Enemy damaged! HP: {hp}");
        CameraShake.instance.ShakeCamera(5f, 0.15f);

        SetHP();

        if (hp <= 0)
        {
            StartCoroutine(Cor_Die());
        }
    }

    protected override void FollowPlayer()
    {
        Vector2 dir = (playerTransform.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);

        if (dir.x < 0)
        {
            spriteren.flipX = false;
            busterEffect.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            busterEffect.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            spriteren.flipX = true;
        }
    }

    protected override IEnumerator Cor_Die()
    {
        isDying = true;

        Vector2 knockbackDir = (transform.position - GameManager.instance.playerCont.transform.position).normalized;
        Vector2 finalKnockback = (knockbackDir + new Vector2(0, 1f)).normalized;
        rb.AddForce(finalKnockback * 11f, ForceMode2D.Impulse);

        hpSlider.gameObject.SetActive(false);
        baseHpSlider.gameObject.SetActive(false);
        StopCoroutine(hitCoroutine);
        spriteren.material = hitMaterial;

        yield return new WaitForSeconds(0.6f);

        SpawnRandomItem(transform.position);
        CameraShake.instance.ShakeCamera(7f, 0.2f);
        ObjectPool.SpawnFromPool("DieEffect", transform.position);
        EXPSpawn();
        StartCoroutine(Cor_TimdSlow());
        spriteren.material = originalMaterial;
        collider.isTrigger = true;
        rb.gravityScale = 2f;

        AudioManager.instance?.PlaySound(transform.position, "EnemyDie", Random.Range(1.4f, 1.4f), 1f);
        AudioManager.instance?.PlaySound(transform.position, "Boom", Random.Range(1f, 1.1f), 1f);
        OnDeath?.Invoke(this);
        SongManager.instance.SongChange(0);

        Destroy(door);
        yield return new WaitForSecondsRealtime(2.2f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      
       
    }
    protected override IEnumerator Cor_TimdSlow()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isDashing = false;
        }
        if (collision.gameObject.CompareTag("Player") && isDashing)
        {
            collision.gameObject.GetComponent<PlayerBase>().TakeDamage(10);
        }
    }
    protected override void SpawnRandomItem(Vector2 spawnPos)
    {
        Instantiate(skillItem, spawnPos, Quaternion.identity);
    }
}
