using BattleRobo;
using Photon;
using UnityEngine;

public class NetworkCommandScript : PunBehaviour
{
    [SerializeField]
    private CommandDispatcherScript commandDispatcherScript;

    private const int IdShift = 1;

    public RoboControllerScript.PlayerState playerState = new RoboControllerScript.PlayerState();
    public RoboControllerScript.PlayerState previousPlayerState = new RoboControllerScript.PlayerState();

    private int index;
    private int currentIndex = -1;

    //mouse input
    private Vector3 mouseInput;

    //player movement
    private float inputX;
    private float inputY;
    private bool isJumping;

    //player actions
    private bool isPausing;
    private bool isFiring;
    private bool isLooting;
    private bool isDropping;

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
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        //player movement
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        isJumping = Input.GetButton("Jump");

        //player actions
        isPausing = Input.GetKeyDown(KeyCode.Escape);
        isFiring = Input.GetButtonDown("Fire1");
        isLooting = Input.GetButtonDown("Loot");
        isDropping = Input.GetButtonDown("Drop");

        playerState = new RoboControllerScript.PlayerState(inputX, inputY, isJumping, mouseInput);

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
            PhotonNetwork.RPC(photonView, "SwitchWeaponRPC", PhotonTargets.AllViaServer, false, PhotonNetwork.player, index);
            currentIndex = index;
        }
    }

    private void FixedUpdate()
    {
        if (!playerState.IsEqual(previousPlayerState))
        {
            PhotonNetwork.RPC(photonView, "MovementRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player, playerState.inputX, playerState.inputY, playerState.isJumping, playerState.mouseInput);

            previousPlayerState = playerState;
        }
    }

    [PunRPC]
    public void MovementRPC(PhotonPlayer player, float x, float y, bool jumping, Vector2 mouse)
    {
        commandDispatcherScript.Movement(player.ID - IdShift, x, y, jumping, mouse);
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
    public void SwitchWeaponRPC(PhotonPlayer player, int id)
    {
        commandDispatcherScript.SwitchWeapon(player.ID - IdShift, id);
    }
}