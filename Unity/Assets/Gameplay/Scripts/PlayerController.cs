using System;
using UnityEngine;


public class PlayerController : Photon.MonoBehaviour, IPunObservable
{
    [Header("Mouse Settings")] [SerializeField] private float aimSensitivity = 5f;

    [Header("Movement Settings")] [SerializeField] private float walkSpeed = 6.0f;

    [SerializeField] private float runSpeed = 11.0f;

    [SerializeField] private float gravity = 20.0f;

    [SerializeField] private float fuelDecreaseSpeed = 0.1f;

    [SerializeField] private float fuelRegenSpeed = 0.05f;

    [SerializeField] private float flyForce = 100f;

    [Header("Required Components")] [SerializeField] private CharacterController controller;

    [SerializeField] private Transform camTransform;

    /// <summary>
    /// The camera gameObject for the shooting.
    /// </summary>
    [SerializeField] private GameObject playerCamera;

    private Vector3 moveDirection = Vector3.zero;
    private bool grounded;
    private float speed;
    private Vector3 fly;

    public float fuelAmount = 1f;
    private float maxFuelAmount = 1f;
    public float currentRot;

    public PlayerState playerState = new PlayerState();

    private Transform myTransform;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        myTransform = transform;
        speed = walkSpeed;

        PhotonNetwork.sendRate = 30;
        PhotonNetwork.sendRateOnSerialize = 30;
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(currentRot);
        }
        else
        {
            currentRot = (float)stream.ReceiveNext();
        }
    }

    private void FixedUpdate()
    {
        // Limit the diagonal speed
        float inputModifyFactor = Math.Abs(playerState.inputX) > 0.0001f && Math.Abs(playerState.inputY) > 0.0001f
            ? .7071f
            : 1.0f;

        // Handle the movement
        if (grounded)
        {
            speed = Input.GetButton("Run") ? runSpeed : walkSpeed;

            moveDirection = new Vector3(playerState.inputX * inputModifyFactor, 0f,
                playerState.inputY * inputModifyFactor);
            moveDirection = myTransform.TransformDirection(moveDirection) * speed;

            // Jump!
            Jump();
        }
        else
        {
            moveDirection.x = playerState.inputX * speed * inputModifyFactor;
            moveDirection.z = playerState.inputY * speed * inputModifyFactor;
            moveDirection = myTransform.TransformDirection(moveDirection);

            // Jump!
            Jump();
        }

        // Rotate the player on the Y axis
        myTransform.rotation *= Quaternion.Euler(0, playerState.mouseInput.x * aimSensitivity, 0);
        
        // Rotate the player on the X axis
        currentRot -= playerState.mouseInput.y * aimSensitivity;
        currentRot = Mathf.Clamp(currentRot, -60f, 60f);
        
        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller, and set grounded true or false depending on whether we're standing on something
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

    private void Update()
    {
        fly = Vector3.zero;
        if (playerState.isJumping && fuelAmount > 0f)
        {
            fuelAmount -= fuelDecreaseSpeed * Time.deltaTime;
            var consumedFuel = maxFuelAmount - fuelAmount;

            if (fuelAmount >= 0.15f)
            {
                fly = Vector3.up * flyForce * consumedFuel;
            }
        }
        else
        {
            fuelAmount += fuelRegenSpeed * Time.deltaTime;
            maxFuelAmount = fuelAmount;
        }

        fuelAmount = Mathf.Clamp(fuelAmount, 0f, 1f);
    }

    private void Jump()
    {
        if (fly != Vector3.zero)
        {
            moveDirection.y = fly.y;
        }
    }

    public void ClientMovement(Vector2 mouseIn, float inputX, float inputY, bool isJumping)
    {
        playerState = new PlayerState(inputX, inputY, isJumping, mouseIn);
    }

    public void SetUp()
    {
        //activate camera only for this player
        playerCamera.SetActive(true);
    }

    [Serializable]
    public class PlayerState
    {
        public float inputX;

        public float inputY;

        public bool isJumping;

        public Vector2 mouseInput;

        public PlayerState()
        {
            inputX = 0f;
            inputY = 0f;
            isJumping = false;
            mouseInput = Vector3.zero;
        }

        public PlayerState(float inputX, float inputY, bool isJumping, Vector2 mouseInput)
        {
            this.inputX = inputX;
            this.inputY = inputY;
            this.isJumping = isJumping;
            this.mouseInput = mouseInput;
        }

        public bool IsEqual(PlayerState other)
        {
            return inputX.Equals(other.inputX) && inputY.Equals(other.inputY) &&
                   isJumping.Equals(other.isJumping) && mouseInput == other.mouseInput;
        }
    }
}