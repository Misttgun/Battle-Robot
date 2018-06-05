using BattleRobo;
using UnityEngine;

public class CommandDispatcherScript : MonoBehaviour
{
    [SerializeField]
    public RoboControllerScript[] PlayerControllersScript;

    public void SetUp(int playerId)
    {
        if (playerId < 0 || playerId >= PlayerControllersScript.Length)
        {
            return;
        }

        PlayerControllersScript[playerId].SetUp();
    }

    public void Movement(int playerId, float inputX, float inputY, bool isJumping, Vector2 mouseInput)
    {
        if (playerId < 0 || playerId >= PlayerControllersScript.Length)
        {
            return;
        }

        PlayerControllersScript[playerId].ClientMovement(inputX, inputY, isJumping, mouseInput);
    }

    public void Shoot(int playerId)
    {
        if (playerId < 0 || playerId >= PlayerControllersScript.Length)
        {
            return;
        }

        PlayerControllersScript[playerId].ClientShooting();
    }

    public void Pause(int playerId)
    {
        if (playerId < 0 || playerId >= PlayerControllersScript.Length)
        {
            return;
        }

        PlayerControllersScript[playerId].ClientPause();
    }
    
    public void Loot(int playerId)
    {
        if (playerId < 0 || playerId >= PlayerControllersScript.Length)
        {
            return;
        }

        PlayerControllersScript[playerId].ClientLoot();
    }
    
    public void Drop(int playerId)
    {
        if (playerId < 0 || playerId >= PlayerControllersScript.Length)
        {
            return;
        }

        PlayerControllersScript[playerId].ClientDrop();
    }

    public void SwitchWeapon(int playerId, int index)
    {
        if (playerId < 0 || playerId >= PlayerControllersScript.Length)
        {
            return;
        }

        PlayerControllersScript[playerId].ClientSwitchWeapon(index);
    }
}