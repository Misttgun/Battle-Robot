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
    
    // game state
    private bool isInPause;

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
        isPausing = Input.GetButtonDown("Pause");
        isFiring = Input.GetButtonDown("Fire1");
        isLooting = Input.GetButtonDown("Loot");
        isDropping = Input.GetButtonDown("Drop");

        // - game state
        isInPause = GameManagerScript.GetInstance().IsGamePause();
        
        
        playerState = new RoboControllerScript.PlayerState(inputX, inputY, isJumping, mouseInput);
        

        if (isFiring && !isInPause)
        {
            PhotonNetwork.RPC(photonView, "ShootingRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player);
        }

        if (isPausing)
        {
            Debug.Log("IsPausing");
            PhotonNetwork.RPC(photonView, "PauseRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player);
        }

        if (isLooting && !isInPause)
        {
            PhotonNetwork.RPC(photonView, "LootRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player);
        }

        if (isDropping && !isInPause)
        {
            PhotonNetwork.RPC(photonView, "DropRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player);
        }

        if (Input.GetButtonDown("Inventory1") && !isInPause)
        {
            index = 0;
        }
        else if (Input.GetButtonDown("Inventory2") && !isInPause)
        {
            index = 1;
        }
        else if (Input.GetButtonDown("Inventory3") && !isInPause)
        {
            index = 2;
        }
        else if (Input.GetButtonDown("Inventory4") && !isInPause)
        {
            index = 3;
        }
        else if (Input.GetButtonDown("Inventory5") && !isInPause)
        {
            index = 4;
        }

        if (currentIndex != index && !isInPause)
        {
            PhotonNetwork.RPC(photonView, "SwitchWeaponRPC", PhotonTargets.AllViaServer, false, PhotonNetwork.player, index);
            currentIndex = index;
        }
    }

    private void FixedUpdate()
    {
        if (!playerState.IsEqual(previousPlayerState) && !GameManagerScript.GetInstance().IsGamePause())
        {
            PhotonNetwork.RPC(photonView, "MovementRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player, playerState.inputX, playerState.inputY, playerState.isJumping, playerState.mouseInput);

            previousPlayerState = playerState;
        }
    }

    private void PauseTimeout()
    {
        // - the master client shall do the Timeout RPC to avoid that a corrupted client keep game in pause forever
        bool isMasterClient = PhotonNetwork.player.IsMasterClient;
        bool isStillInPause = GameManagerScript.GetInstance().IsGamePause();
   
        // - if still in pause, master client will send an RPC to exit pause
        if (isMasterClient && isStillInPause)
            PhotonNetwork.RPC(photonView, "PauseTimeoutRPC", PhotonTargets.MasterClient, false, PhotonNetwork.player);
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
        int counter = 0;
        bool isGamePause = GameManagerScript.GetInstance().IsGamePause();
        bool found = GameManagerScript.GetInstance().pauseCounter.TryGetValue(player.ID, out counter);

        if (!isGamePause)
        {
            // - set current player in pause
            GameManagerScript.GetInstance().SetPlayerInPause(player.ID);
                
            // - increment pause counter
            if (found)
                GameManagerScript.GetInstance().pauseCounter[player.ID]++;

            // - init pause counter if necessary
            else
                GameManagerScript.GetInstance().pauseCounter[player.ID] = 0;
            
            // - max pause duration is 10 sec
            Invoke("PauseTimeout", 10);
        }
        
        // - dispatch pause if the player havn't used it more than 3 times or if the game is already in pause
        if (isGamePause && GameManagerScript.GetInstance().GetPlayerInPause() == player.ID || (!isGamePause && counter < 2))
            commandDispatcherScript.Pause(player.ID - IdShift);
    }

    [PunRPC]
    public void PauseTimeoutRPC(PhotonPlayer player)
    {
        commandDispatcherScript.PauseTimeout(player.ID - IdShift);
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