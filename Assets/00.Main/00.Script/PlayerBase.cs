using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AttackType
{
    Gun,
    Sword
}
public class PlayerBase : MonoBehaviour
{
    public AttackType currentAttackType;
    [Header("Stat Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxHp;
    [SerializeField] private float currentHp;
    public float jumpForce = 10f;
    public int maxJumpCount = 2;
    public int currentJumpCount;
    public GameObject whirlwind;


    [Header("Dash Settings")]
    public float dashForce = 20f;       // 대시 힘
    public float dashDuration = 0.2f;   // 대시 지속 시간
    public float dashCooldown = 1f;     // 대시 쿨타임
    private bool isDashing = false;     // 현재 대시 중인지 여부
    private bool canDash = true;        // 대시 가능 여부
    private bool isWind = false;

    [Header("Combat Settings")]
    public Transform firePoint;
    public GameObject laserPrefab;
    public PlayerAttack playerAttack;

    private IPlayerState currentState;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    public Rigidbody2D rb;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Color gunColor = Color.white;
    [SerializeField] private Color swordColor = Color.blue;

    private Coroutine ghostCoroutine;
    public bool IsGrounded
    {
        get
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.2f, groundLayer);
            Debug.DrawRay(groundCheck.position, Vector2.down * 0.2f, Color.red);
            return hit.collider != null;
        }
    }

    [Header("Ghost Effect")]
    [SerializeField] private Ghost ghost;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        ChangeState(new PlayerIdleState());
        currentJumpCount = maxJumpCount;

        currentHp = maxHp;

        hpSlider.value = currentHp / maxHp;
        hpText.text = $"{currentHp}/{maxHp}";
    }


    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.transform.position.z * -1f;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector2 dir = (mouseWorldPos - transform.position).normalized;

        Flip(dir.x);

        if (isDashing && !IsGrounded && !isWind && currentJumpCount == 0)
        {
            StartCoroutine(Cor_DashJumpWind());
        }

        if(currentAttackType == AttackType.Gun)
        {
            if (Input.GetMouseButtonDown(0) && currentState is PlayerIdleState)
            {
                playerAttack.Fire(dir);
                anim.SetTrigger("Shot");
            }
            else if (Input.GetMouseButtonDown(0))
            {
                playerAttack.Fire(dir);
            }
            if (Input.GetMouseButtonDown(1))
            {
                playerAttack.SkillLaserFire();
            }
        }
        else if(currentAttackType == AttackType.Sword)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SwordAttack();
            }
            if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(playerAttack.SwordSkill());
            }
        }
        
        if (IsGrounded && currentJumpCount != maxJumpCount)
        {
            currentJumpCount = maxJumpCount;
            Debug.Log("착지 - 점프 횟수 초기화");
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount > 0)
        {
            ChangeState(new PlayerJumpState());
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeWeapon();
        }
        // 방향 입력
        float inputX = Input.GetAxisRaw("Horizontal");

        // 대시 입력
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing && inputX != 0)
        {
            StartCoroutine(Dash(inputX));
            GhostSpawn();
        }


        currentState?.Update(this);
    }


    void SwordAttack()
    {
        playerAttack.Damage(playerAttack.attackPos, playerAttack.attackBoxSize);
        PushForward(2f);
        AudioManager.instance?.PlaySound(transform.position, "Sword", Random.Range(1f, 1.2f), 1f);

        playerAttack.SwordSlashToggle();
    }

    private IEnumerator Dash(float direction)
    {
        isDashing = true;
        canDash = false;

        rb.velocity = Vector2.zero; // 기존 속도 제거
        rb.AddForce(Vector2.right * direction * dashForce, ForceMode2D.Impulse);

        anim.SetTrigger("Dash");

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector2.zero; // 대시 끝나고 정지
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public void AnimationTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    public void AnimationBool(string name, bool bl)
    {
        anim.SetBool(name, bl);
    }

    public void Move(float dir)
    {
        transform.Translate(Vector3.right * dir * moveSpeed * Time.deltaTime);
    }

    private void Flip(float dir)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (dir < 0 ? -1 : 1);
        transform.localScale = scale;
    }


    public void GhostSpawn()
    {
        if (ghostCoroutine != null)
        {
            StopCoroutine(ghostCoroutine); // 이전 코루틴 중단
        }

        ghostCoroutine = StartCoroutine(GhostCor());
    }

    private IEnumerator GhostCor()
    {
        ghost.ghostDelay /= 2;
        ghost.makeGhost = true;

        yield return new WaitForSeconds(0.16f);

        ghost.ghostDelay *= 2;
        ghost.makeGhost = false;

        ghostCoroutine = null; // 코루틴 종료 처리
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        CameraShake.instance.ShakeCamera(5f, 0.15f);
        hpSlider.value = currentHp/maxHp;
        hpText.text = $"{currentHp}/{maxHp}";
    }
    private void ChangeWeapon()
    {
        GameManager.instance.Flash();
        AudioManager.instance?.PlaySound(transform.position, "Change", Random.Range(1.4f, 1.4f), 1f);

        if (currentAttackType == AttackType.Gun)
        {
            currentAttackType = AttackType.Sword;
            spriteRenderer.color = swordColor;
            SetBigLaserSliderColor(swordColor);
        }
        else
        {
            currentAttackType = AttackType.Gun;
            spriteRenderer.color = gunColor;
            SetBigLaserSliderColor(gunColor);
        }
    }

    private void SetBigLaserSliderColor(Color color)
    {
        Image fillImage = playerAttack.bigLaserValueSlider.fillRect.GetComponent<Image>();
        if (fillImage != null)
        {
            fillImage.color = color;
        }
    }

    IEnumerator Cor_DashJumpWind()
    {
        AudioManager.instance?.PlaySound(transform.position, "휠윈드", Random.Range(1f, 1.2f), 1f);
        isWind = true;
        whirlwind.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            playerAttack.Damage(playerAttack.windAttackPos, playerAttack.windAttackBoxSize);
            yield return new WaitForSeconds(0.05f);
        }
        isWind = false;
        whirlwind.SetActive(false);
    }

    public void PushForward(float force = 3f)
    {
        Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
    }

   

}
