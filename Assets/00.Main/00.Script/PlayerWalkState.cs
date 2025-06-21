using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerWalkState : IPlayerState
{
    public void Enter(PlayerBase player)
    {
        Debug.Log("Walk ���� ����");
        player.AnimationBool("Move", true);
    }

    public void Update(PlayerBase player)
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        if (inputX == 0)
        {
            player.ChangeState(new PlayerIdleState());
            return;
        }

        player.Move(inputX); // �̵� ó��
    }

    public void Exit(PlayerBase player) {
        Debug.Log("Walk ���� ����");
    }

}
