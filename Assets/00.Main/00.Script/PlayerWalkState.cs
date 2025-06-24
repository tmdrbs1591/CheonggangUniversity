using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerWalkState : IPlayerState
{
    public void Enter(PlayerBase player)
    {
        if (DialogManager.instance.isDialogActive)
            return;
        Debug.Log("Walk 상태 진입");
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

        player.Move(inputX); // 이동 처리
    }

    public void Exit(PlayerBase player) {
        Debug.Log("Walk 상태 종료");
    }

}
