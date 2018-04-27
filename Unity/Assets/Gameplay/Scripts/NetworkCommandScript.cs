using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class NetworkCommandScript : PunBehaviour
{
    [SerializeField] private CommandDispatcherScript commandDispatcherScript;

    private const int IdShift = 1;

    public PlayerController.PlayerState playerState = new PlayerController.PlayerState();
    public PlayerController.PlayerState previousPlayerState = new PlayerController.PlayerState();

    private void Start()
    {
        PhotonNetwork.RPC(photonView,
            "SetUpRPC",
            PhotonNetwork.player,
            false,
            PhotonNetwork.player);
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

        Vector3 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        bool isJumping = Input.GetButton("Jump");

        playerState = new PlayerController.PlayerState(inputX, inputY, isJumping, mouseInput);

        if (!playerState.IsEqual(previousPlayerState))
        {
            PhotonNetwork.RPC(photonView,
                "MovementRPC",
                PhotonTargets.MasterClient,
                false,
                PhotonNetwork.player,
                playerState.mouseInput,
                playerState.inputX,
                playerState.inputY,
                playerState.isJumping);

            previousPlayerState = playerState;
        }
    }

    [PunRPC]
    public void MovementRPC(PhotonPlayer player, Vector2 mouseInput, float inputX, float inputY, bool isJumping)
    {
        if (PhotonNetwork.isMasterClient)
        {
            commandDispatcherScript.Movement(player.ID - IdShift, mouseInput, inputX, inputY, isJumping);
        }
    }

    [PunRPC]
    public void SetUpRPC(PhotonPlayer player)
    {
        commandDispatcherScript.SetUp(player.ID - IdShift);
    }
}