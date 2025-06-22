using System.Collections;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [Header("Stat Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxHp;
    [SerializeField] private float currentHp;
    public float jumpForce = 10f;
    public int maxJumpCount = 2;
    public int currentJumpCount;

    [Header("Dash Settings")]
    public float dashForce = 20f;       // 대시 힘
    public float dashDuration = 0.2f;   // 대시 지속 시간
    public float dashCooldown = 1f;     // 대시 쿨타임
    private bool isDashing = false;     // 현재 대시 중인지 여부
    private bool canDash = true;        // 대시 가능 여부

    [Header("Combat Settings")]
    public Transform firePoint;
    public GameObject laserPrefab;
    public PlayerAttack playerAttack;

    // ==========================
    // 🔹 Components
    // ==========================
    private IPlayerState currentState;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    public Rigidbody2D rb;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;


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
    }


    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.transform.position.z * -1f;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector2 dir = (mouseWorldPos - transform.position).normalized;

        Flip(dir.x);

        if (Input.GetMouseButtonDown(0) && currentState is PlayerIdleState)
        {
            playerAttack.Fire(dir);
            anim.SetTrigger("Shot");
        }
        else if (Input.GetMouseButtonDown(0))
        {
            playerAttack.Fire(dir);
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
        spriteRenderer.flipX = dir < 0;
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

    }

}
