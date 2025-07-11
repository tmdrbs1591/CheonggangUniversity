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



    private void Start()
    {
        baseHpSlider.gameObject.SetActive(false);
        SetHP();
        base.Start();
    }

    private void Update()
    {
        if (isDying || playerTransform == null) return;

        if (TimeLineManager.instance.isCutScene || !isBattle ) return;
        base.Update();

        baseHpSlider.value = Mathf.Lerp(baseHpSlider.value, targetValue, Time.deltaTime * 4f);
        hpSlider.value = Mathf.Lerp(hpSlider.value, targetValue, Time.deltaTime * 8f);

    }


    protected override void FollowPlayer()
    {

    }



    protected override IEnumerator Cor_Attack()
    {
    
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
