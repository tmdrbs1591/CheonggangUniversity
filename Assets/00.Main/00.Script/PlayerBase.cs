using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    private IPlayerState currentState;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        ChangeState(new PlayerIdleState());
    }

    void Update()
    {
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
    public void AnimationBool(string name,bool bl)
    {
        anim.SetBool(name,bl);
    }
    public void Move(float dir)
    {
        transform.Translate(Vector3.right * dir * moveSpeed * Time.deltaTime);
        Flip(dir);
    }

    private void Flip(float dir)
    {
        if (dir > 0)
            transform.localScale = new Vector3(4, 4, 4);  // 기본 스케일
        else if (dir < 0)
            transform.localScale = new Vector3(-4, 4, 4); // x축 반전
    }


}
