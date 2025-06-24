using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerWalkState : IPlayerState
{
    public void Enter(PlayerBase player)
    {
        if (DialogManager.instance.isDialogActive)
            return;
        Debug.Log("Walk ���� ����");
        player.AnimationBool("Move", true);
    }

    public void Update(PlayerBase player)
    {
        if (DialogManager.instance.isDialogActive)
            return;
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
