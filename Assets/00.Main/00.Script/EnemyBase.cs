using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Stat")]
    [SerializeField] protected float maxHp = 10;
    [SerializeField] protected float hp = 10;


    [Header("Component")]
    [SerializeField] protected Slider hpSlider;
    [SerializeField] protected LineRenderer dangerLineRenderer;
    [SerializeField] protected GameObject dangerLine;
    [SerializeField] protected float lineDuration = 0.2f;

    [SerializeField] protected Material hitMaterial;
    [SerializeField] protected Material originalMaterial;
    [SerializeField] protected SpriteRenderer spriteren;

    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Collider2D collider;

    protected Coroutine hitCoroutine;

    public bool isDying { get; protected set; }

    [Header("AI")]
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected float attackRange = 3f;
    [SerializeField] protected float moveSpeed = 3f;

    [SerializeField] protected float attackCoolTime = 2f; // 공격 쿨타임
    protected float currentCoolTime = 0f;

    protected Transform playerTransform;
    protected bool isAttacking = false;

    public void Start()
    {
        hp = maxHp;
        playerTransform = GameManager.instance.playerCont.transform;
    }

    public void Update()
    {
        if (isDying || playerTransform == null) return;

        // 쿨타임 진행
        if (currentCoolTime > 0f)
            currentCoolTime -= Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)   
        {
            FollowPlayer();
        }
        else if (distanceToPlayer <= attackRange && !isAttacking && currentCoolTime <= 0f)
        {
            StartCoroutine(Cor_Attack());
        }
    }

    protected virtual void FollowPlayer()
    {
        Vector2 dir = (playerTransform.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);

        // Sprite Flip
        if (dir.x < 0)
            spriteren.flipX = true;
        else
            spriteren.flipX = false;
    }

    protected virtual IEnumerator Cor_Attack()
    {
        yield return new WaitForSeconds(1f);
    }

    public virtual void TakeDamage(float amount)
    {
    
    }

    protected virtual IEnumerator Cor_Die()
    {
        isDying = true;

        if (playerTransform != null)
        {
            Vector2 knockbackDir = (transform.position - playerTransform.position).normalized;
            Vector2 finalKnockback = (knockbackDir + new Vector2(0, 1f)).normalized;

            rb.AddForce(finalKnockback * 11f, ForceMode2D.Impulse);
        }

        hpSlider.gameObject.SetActive(false);

        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        spriteren.material = hitMaterial;

        yield return new WaitForSeconds(0.6f);

        CameraShake.instance.ShakeCamera(7f, 0.2f);
        ObjectPool.SpawnFromPool("DieEffect", transform.position);
        EXPSpawn();
        spriteren.material = originalMaterial;
        collider.isTrigger = true;
        AudioManager.instance?.PlaySound(transform.position, "EnemyDie", Random.Range(1.4f, 1.4f), 1f);
        AudioManager.instance?.PlaySound(transform.position, "Boom", Random.Range(1f, 1.1f), 1f);

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }

    protected IEnumerator Cor_HitMaterialChange()
    {
        spriteren.material = hitMaterial;

        yield return new WaitForSeconds(0.2f);

        spriteren.material = originalMaterial;

        hitCoroutine = null;
    }

    protected void EXPSpawn()
    {
        int ranVal = Random.Range(5, 8);
        for (int i = 0; i < ranVal; i++)
        {
            ObjectPool.SpawnFromPool("EXP", transform.position);
        }
    }
}
