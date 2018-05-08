using System.Runtime.CompilerServices;
using BattleRobo;
using Photon;
using UnityEngine;

public class NetworkCommandScript : PunBehaviour
{
    [SerializeField]
    private CommandDispatcherScript commandDispatcherScript;

    private const int IdShift = 1;

    public RoboController.PlayerState playerState = new RoboController.PlayerState();
    public RoboController.PlayerState previousPlayerState = new RoboController.PlayerState();

    private int index;
    private int currentIndex = -1;

    private void Start()
    {
        PhotonNetwork.RPC(photonView, "SetUpRPC", PhotonNetwork.player, false, PhotonNetwork.player);
    }

    private void Update()
    {
        // Cursor lock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //mouse input
        Vector3 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        //player movement
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        bool isJumping = Input.GetButton("Jump");
        bool isSpriting = Input.GetButton("Run");

        //player actions
        bool isPausing = Input.GetKeyDown(KeyCode.Escape);
        bool isFiring = Input.GetButtonDown("Fire1");
        bool isLooting = Input.GetButtonDown("Loot");
        bool isDropping = Input.GetButtonDown("Drop");

        playerState = new RoboController.PlayerState(inputX, inputY, isJumping, isSpriting, mouseInput);

        if (!playerState.IsEqual(previousPlayerState))
        {
            PhotonNetwork.RPC(photonView, "MovementRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player, playerState.inputX, playerState.inputY, playerState.isJumping, playerState.isSpriting, playerState.mouseInput);

            previousPlayerState = playerState;
        }

        if (isFiring)
        {
            PhotonNetwork.RPC(photonView, "ShootingRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player);
        }

        if (isPausing)
        {
            PhotonNetwork.RPC(photonView, "PauseRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player);
        }

        if (isLooting)
        {
            PhotonNetwork.RPC(photonView, "LootRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player);
        }

        if (isDropping)
        {
            PhotonNetwork.RPC(photonView, "DropRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player);
        }

        if (Input.GetButtonDown("Inventory1"))
        {
            index = 0;
        }
        else if (Input.GetButtonDown("Inventory2"))
        {
            index = 1;
        }
        else if (Input.GetButtonDown("Inventory3"))
        {
            index = 2;
        }
        else if (Input.GetButtonDown("Inventory4"))
        {
            index = 3;
        }
        else if (Input.GetButtonDown("Inventory5"))
        {
            index = 4;
        }

        if (currentIndex != index)
        {
            PhotonNetwork.RPC(photonView, "SwitchWeaponRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player, index);
            currentIndex = index;
        }
    }

    [PunRPC]
    public void MovementRPC(PhotonPlayer player, float inputX, float inputY, bool isJumping, bool isSpriting, Vector2 mouseInput)
    {
        commandDispatcherScript.Movement(player.ID - IdShift, inputX, inputY, isJumping, isSpriting, mouseInput);
    }

    [PunRPC]
    public void SetUpRPC(PhotonPlayer player)
    {
        commandDispatcherScript.SetUp(player.ID - IdShift);
    }

    [PunRPC]
    public void ShootingRPC(PhotonPlayer player)
    {
        if (PhotonNetwork.isMasterClient)
        {
            commandDispatcherScript.Shoot(player.ID - IdShift);
        }
    }

    [PunRPC]
    public void PauseRPC(PhotonPlayer player)
    {
        commandDispatcherScript.Pause(player.ID - IdShift);
    }

    [PunRPC]
    public void LootRPC(PhotonPlayer player)
    {
        commandDispatcherScript.Loot(player.ID - IdShift);
    }

    [PunRPC]
    public void DropRPC(PhotonPlayer player)
    {
        commandDispatcherScript.Drop(player.ID - IdShift);
    }

    [PunRPC]
    public void SwitchWeaponRPC(PhotonPlayer player, int index)
    {
        commandDispatcherScript.SwitchWeapon(player.ID - IdShift, index);
    }
}