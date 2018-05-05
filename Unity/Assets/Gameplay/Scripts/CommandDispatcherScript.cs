using System.Collections;
using System.Collections.Generic;
using BattleRobo;
using UnityEngine;

public class CommandDispatcherScript : MonoBehaviour
{
    [SerializeField] private RoboController[] playerControllers;
    
    public void SetUp(int playerId)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].SetUp();
    }

    public void Movement(int playerId,float inputX, float inputY, bool isJumping, bool isSpriting, Vector2 mouseInput)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].ClientMovement(inputX, inputY, isJumping, isSpriting, mouseInput);
    }
    
    public void Shoot(int playerId, bool isFiring)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].ClientShooting(isFiring);
    }

    public void Pause(int playerId, bool isPausing)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].ClientPause(isPausing);
    }
}