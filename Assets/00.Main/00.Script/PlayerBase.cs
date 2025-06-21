using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    public Transform firePoint;
    public GameObject laserPrefab;
    public PlayerAttack playerAttack;

    private IPlayerState currentState;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        ChangeState(new PlayerIdleState());
    }

    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.transform.position.z * -1f;  // z°ª º¸Á¤
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector2 dir = (mouseWorldPos - transform.position).normalized;

        Flip(dir.x);

        if (Input.GetMouseButtonDown(0))
        {
            playerAttack.Fire(dir);
            anim.SetTrigger("Attack");
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
}
