using System.Collections;
using System.Collections.Generic;
using BattleRobo;
using UnityEngine;

public class CommandDispatcherScript : MonoBehaviour
{
    [SerializeField]
    public RoboController[] playerControllers;

    public void SetUp(int playerId)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].SetUp();
    }

    public void Movement(int playerId, float inputX, float inputY, bool isJumping, bool isSpriting, Vector2 mouseInput)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].ClientMovement(inputX, inputY, isJumping, isSpriting, mouseInput);
    }

    public void Shoot(int playerId)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].ClientShooting();
    }

    public void Pause(int playerId)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].ClientPause();
    }
    
    public void Loot(int playerId)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].ClientLoot();
    }
    
    public void Drop(int playerId)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].ClientDrop();
    }

    public void SwitchWeapon(int playerId, int index)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].ClientSwitchWeapon(index);
    }
}