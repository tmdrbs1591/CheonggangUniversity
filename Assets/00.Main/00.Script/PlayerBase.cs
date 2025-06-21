using System.Collections;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Ghost ghost;
    public float jumpForce;
    public int maxJumpCount = 2;     // 최대 점프 횟수 (2단점프)
    public int currentJumpCount;

    public Transform firePoint;
    public GameObject laserPrefab;
    public PlayerAttack playerAttack;

    private IPlayerState currentState;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    public Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded
    {
        get
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.2f, groundLayer);
            Debug.DrawRay(groundCheck.position, Vector2.down * 0.2f, Color.red);
            return hit.collider != null;
        }
    }


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        ChangeState(new PlayerIdleState());
        currentJumpCount = maxJumpCount;

    }

    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.transform.position.z * -1f;  // z값 보정
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

        currentState?.Update(this);
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

    public void  GhostSpawn()
    {
        StartCoroutine(GhostCor());
    }
    private IEnumerator GhostCor()
    {
        ghost.ghostDelay /= 2;
        ghost.makeGhost = true;

        yield return new WaitForSeconds(0.16f);

        ghost.ghostDelay *= 2;
        ghost.makeGhost = false;
    }

}
