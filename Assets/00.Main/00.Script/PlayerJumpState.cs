using UnityEngine;
using System.Collections;


public class PlayerJumpState : IPlayerState
{
    public void Enter(PlayerBase player)
    {
        Debug.Log("����!");

        player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpForce);

        // ���� ī��Ʈ ����

        // 2�� ������ DoubleJump �ִϸ��̼� ����
        if (player.currentJumpCount == 0)
        {
            player.AnimationTrigger("Jump"); // 2�� ���� �ִϸ��̼� 
            player.GhostSpawn();
        }

   
    }

    public void Update(PlayerBase player)
    {
        float moveDir = Input.GetAxisRaw("Horizontal");
        player.Move(moveDir);

        // �̴� ���� ����
        if (Input.GetKeyDown(KeyCode.Space) && player.currentJumpCount > 0)
        {
            player.ChangeState(new PlayerJumpState());
        }

        // ���� ����
        if (player.IsGrounded && player.rb.velocity.y <= 0.1f)
        {
            player.ChangeState(new PlayerIdleState());
        }
    }

    public void Exit(PlayerBase player) { 
        player.currentJumpCount--;
    }
}
