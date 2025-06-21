using UnityEngine;
using System.Collections;


public class PlayerJumpState : IPlayerState
{
    public void Enter(PlayerBase player)
    {
        Debug.Log("점프!");

        player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpForce);

        // 점프 카운트 감소

        // 2단 점프면 DoubleJump 애니메이션 실행
        if (player.currentJumpCount == 0)
        {
            player.AnimationTrigger("Jump"); // 2단 점프 애니메이션 
            player.GhostSpawn();
        }

   
    }

    public void Update(PlayerBase player)
    {
        float moveDir = Input.GetAxisRaw("Horizontal");
        player.Move(moveDir);

        // 이단 점프 조건
        if (Input.GetKeyDown(KeyCode.Space) && player.currentJumpCount > 0)
        {
            player.ChangeState(new PlayerJumpState());
        }

        // 착지 판정
        if (player.IsGrounded && player.rb.velocity.y <= 0.1f)
        {
            player.ChangeState(new PlayerIdleState());
        }
    }

    public void Exit(PlayerBase player) { 
        player.currentJumpCount--;
    }
}
