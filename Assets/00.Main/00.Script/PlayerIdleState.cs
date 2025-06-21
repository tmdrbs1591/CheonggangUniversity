using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    public void Enter(PlayerBase player)
    {
        Debug.Log("Idle ���� ����");
        player.AnimationBool("Move", false);
    }

    public void Update(PlayerBase player)
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        if (inputX != 0)
            player.ChangeState(new PlayerWalkState());
    }

    public void Exit(PlayerBase player) { 
        Debug.Log("Idle ���� ����");
    }
}
