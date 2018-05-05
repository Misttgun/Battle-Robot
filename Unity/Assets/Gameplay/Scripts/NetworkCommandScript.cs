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


        playerState = new RoboController.PlayerState(inputX, inputY, isJumping, isSpriting, mouseInput);

        if (!playerState.IsEqual(previousPlayerState))
        {
            PhotonNetwork.RPC(photonView, "MovementRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player, 
                playerState.inputX, playerState.inputY, playerState.isJumping, playerState.isSpriting, playerState.mouseInput);

            previousPlayerState = playerState;
        }

        if (isFiring)
        {
            PhotonNetwork.RPC(photonView, "ShootingRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player, true);
        }

        if (isPausing)
        {
            PhotonNetwork.RPC(photonView, "PauseRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player, true);
        }
    }

    [PunRPC]
    public void MovementRPC(PhotonPlayer player, float inputX, float inputY, bool isJumping, bool isSpriting, Vector2 mouseInput)
    {
        if (PhotonNetwork.isMasterClient)
        {
            commandDispatcherScript.Movement(player.ID - IdShift, inputX, inputY, isJumping, isSpriting, mouseInput);
        }
    }

    [PunRPC]
    public void SetUpRPC(PhotonPlayer player)
    {
        commandDispatcherScript.SetUp(player.ID - IdShift);
    }

    [PunRPC]
    public void ShootingRPC(PhotonPlayer player, bool isFiring)
    {
        if (PhotonNetwork.isMasterClient)
        {
            commandDispatcherScript.Shoot(player.ID - IdShift, isFiring);
        }
    }

    [PunRPC]
    public void PauseRPC(PhotonPlayer player, bool isPausing)
    {
        commandDispatcherScript.Pause(player.ID - IdShift, isPausing);
    }
}