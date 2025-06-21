using UnityEngine;

public interface IPlayerState
{
    void Enter(PlayerBase player);
    void Update(PlayerBase player);
    void Exit(PlayerBase player);

}