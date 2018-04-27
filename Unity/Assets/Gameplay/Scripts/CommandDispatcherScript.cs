using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDispatcherScript : MonoBehaviour
{
    [SerializeField] private PlayerController[] playerControllers;
    
    public void SetUp(int playerId)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].SetUp();
    }

    public void Movement(int playerId, Vector2 mouseInput, float inputX, float inputY, bool isJumping)
    {
        if (playerId < 0 || playerId >= playerControllers.Length)
        {
            return;
        }

        playerControllers[playerId].ClientMovement(mouseInput, inputX, inputY, isJumping);
    }

}