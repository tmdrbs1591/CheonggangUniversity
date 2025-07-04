using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RiderBoss : EnemyBase
{
    [SerializeField] private float bulletSpeed = 15f;  // 원하는 속도 조절
    [SerializeField] private Slider baseHpSlider;
    [SerializeField] private GameObject busterEffect;
    [SerializeField] private GameObject dashAttackDangerArea;
    private float targetValue = 1f;

    [Header("대시")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashCheckDistance = 1f;
     [SerializeField] private bool isDashing = false;
    private Vector2 dashDirection;
    private void Start()
    {
        baseHpSlider.gameObject.SetActive(false);
        SetHP();
        base.Start();
    }

    private void Update()
    {
        base.Update();
        baseHpSlider.value = Mathf.Lerp(baseHpSlider.value, targetValue, Time.deltaTime * 4f);
        hpSlider.value = Mathf.Lerp(hpSlider.value, targetValue, Time.deltaTime * 8f);
        ;
        if (Input.GetKeyDown(KeyCode.K))
        {
            DashAttack();
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

        // 플레이어 위치 기준으로 좌우만 판단
        float horizontal = playerTransform.position.x - transform.position.x;
        if (horizontal > 0)
            dashDirection = Vector2.right;  // 오른쪽
        else
            dashDirection = Vector2.left;   // 왼쪽

        isDashing = true;
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
            yield return null; // 다음 프레임까지 대기 (실시간 업데이트)
        }

        dangerLineRenderer.enabled = false;

        Debug.Log("공격!");

        // 플레이어 방향으로 불렛 발사
        BulletFire();

        // 쿨타임 시작
        currentCoolTime = attackCoolTime;

        yield return new WaitForSeconds(1f);

        isAttacking = false;
    }
    public void SetHP()
    {
        targetValue = hp/maxHp;
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

        // Sprite Flip
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

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isDashing = false;
        }
    }
}
