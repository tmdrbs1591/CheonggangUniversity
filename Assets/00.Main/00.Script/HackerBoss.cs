using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HackerBoss : EnemyBase
{
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private Slider baseHpSlider;
    [SerializeField] private GameObject skillItem;

    private float targetValue = 1f;
    private bool isDashing = false;
    public bool isBattle = false;

    [SerializeField] GameObject[] dangerLines;
    [SerializeField] Transform[] movePos;

    public Door door;
    private void Start()
    {
        baseHpSlider.gameObject.SetActive(false);
        SetHP();
        base.Start();

    }

    private bool wasBattle = false;

    private void Update()
    {
        if (isDying || playerTransform == null) return;
        if (TimeLineManager.instance.isCutScene) return;

        base.Update();

        baseHpSlider.value = Mathf.Lerp(baseHpSlider.value, targetValue, Time.deltaTime * 4f);
        hpSlider.value = Mathf.Lerp(hpSlider.value, targetValue, Time.deltaTime * 8f);

        // isBattle이 false → true로 바뀌는 순간 감지
        if (!wasBattle && isBattle)
        {
            wasBattle = true;

            // ★ 여기서 한 번만 실행할 코드 작성
            FollowPlayer(); // 예시: 움직임 시작
            StartCoroutine(Cor_Attack()); // 예시: 첫 공격 시작
        }
    }



    private Coroutine moveCoroutine;

    protected override void FollowPlayer()
    {
        if (moveCoroutine == null)
            moveCoroutine = StartCoroutine(Cor_MoveLoop());
    }

    private IEnumerator Cor_MoveLoop()
    {
        while (true)
        {
            if (movePos.Length == 0) yield break;

            Transform target = movePos[Random.Range(0, movePos.Length)];
            Vector3 startPos = transform.position;
            Vector3 endPos = target.position;

            float duration = 0.7f;
            float elapsed = 0f;

            // 부드러운 이동
            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;

            yield return new WaitForSeconds(2f); // 다음 이동까지 대기 (총 3초 주기)
        }
    }


    private int attackCount = 0; // 공격 횟수 카운트

    protected override IEnumerator Cor_Attack()
    {
        if (dangerLines.Length == 0)
            yield break;

        currentCoolTime = attackCoolTime;

        attackCount++;

        if (attackCount >= 5)
        {
            // 전 범위 공격
            foreach (var line in dangerLines)
            {
                if (line == null) continue;

                line.SetActive(true);
            }

            yield return new WaitForSeconds(0.5f); // 경고 시간

            foreach (var line in dangerLines)
            {
                if (line == null) continue;

                line.SetActive(false);

                Vector3 spawnPos = line.transform.position;
                GameObject bulletObj = ObjectPool.SpawnFromPool("HackerBullet", spawnPos);

                if (bulletObj != null)
                {
                    Vector2 shootDir = Vector2.down;
                    Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();
                    if (bulletRb != null)
                    {
                        bulletRb.velocity = shootDir * bulletSpeed;
                    }

                    float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
                    bulletObj.transform.rotation = Quaternion.Euler(0, 0, angle);
                }
            }

            attackCount = 0; // 카운트 초기화
        }
        else
        {
            // 랜덤 한 곳만 공격
            int randIndex = Random.Range(0, dangerLines.Length);
            GameObject targetLine = dangerLines[randIndex];

            targetLine.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            targetLine.SetActive(false);

            Vector3 spawnPos = targetLine.transform.position;
            GameObject bulletObj = ObjectPool.SpawnFromPool("HackerBullet", spawnPos);

            if (bulletObj != null)
            {
                Vector2 shootDir = Vector2.down;
                Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    bulletRb.velocity = shootDir * bulletSpeed;
                }

                float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
                bulletObj.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        yield return new WaitForSeconds(1f);
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

        Destroy(door.gameObject);
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
    protected override void SpawnRandomItem(Vector2 spawnPos)
    {
        Instantiate(skillItem, spawnPos, Quaternion.identity);
    }
}
