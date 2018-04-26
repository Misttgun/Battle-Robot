using System;
using Photon;
using UnityEngine;


public class PlayerController : PunBehaviour
{
    [Header("Mouse Settings")]
    [SerializeField]
    private float aimSensitivity = 5f;

    [Header("Movement Settings")]
    [SerializeField]
    private float walkSpeed = 6.0f;

    [SerializeField]
    private float runSpeed = 11.0f;

    [SerializeField]
    private float gravity = 20.0f;

    [SerializeField]
    private float fuelDecreaseSpeed = 0.1f;

    [SerializeField]
    private float fuelRegenSpeed = 0.05f;

    [SerializeField]
    private float flyForce = 100f;

    [Header("Required Components")]
    [SerializeField]
    private CharacterController controller;

    /// <summary>
    /// The camera gameObject for the shooting.
    /// </summary>
    [SerializeField]
    private GameObject playerCamera;

    [SerializeField]
    private PhotonView myPhotonView;

    private Vector3 moveDirection = Vector3.zero;
    private bool grounded;
    private float speed;

    public float fuelAmount = 1f;
    private float maxFuelAmount = 1f;
    public float currentRot;

    private Transform myTransform;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        myTransform = transform;
        speed = walkSpeed;

        if (!myPhotonView.isMine)
            return;

        //activate camera only for this player
        playerCamera.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (!myPhotonView.isMine)
            return;

        // Rotate the player on the Y axis
        myTransform.rotation *= Quaternion.Euler(0, Input.GetAxisRaw("Mouse X") * aimSensitivity, 0);
    }

    private void Update()
    {
        if (!myPhotonView.isMine)
            return;

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

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        float consumedFuel = 0f;

        if (Input.GetButton("Jump") && fuelAmount > 0f)
        {
            fuelAmount -= fuelDecreaseSpeed * Time.deltaTime;
            consumedFuel = maxFuelAmount - fuelAmount;
        }
        else
        {
            fuelAmount += fuelRegenSpeed * Time.deltaTime;
            maxFuelAmount = fuelAmount;
        }

        myPhotonView.RPC("ClientMovement", PhotonTargets.All, inputX, inputY, consumedFuel, fuelAmount);

        fuelAmount = Mathf.Clamp(fuelAmount, 0f, 1f);
    }

    private void LateUpdate()
    {
        if (!myPhotonView.isMine)
            return;

        // Rotate the player on the X axis
        currentRot -= Input.GetAxisRaw("Mouse Y") * aimSensitivity;
        currentRot = Mathf.Clamp(currentRot, -60f, 60f);
    }

    private void Jump(float fuel, float consFuel)
    {
        Vector3 fly = Vector3.zero;
        if (fuel >= 0.15f)
        {
            fly = Vector3.up * flyForce * consFuel;
        }

        if (fly != Vector3.zero)
        {
            moveDirection.y = fly.y;
        }
    }

    [PunRPC]
    public void ClientMovement(float inputX, float inputY, float consumedFuel, float fuel)
    {
        if (!myPhotonView.isMine)
            return;

        // Limit the diagonal speed
        float inputModifyFactor = Math.Abs(inputX) > 0.0001f && Math.Abs(inputY) > 0.0001f ? .7071f : 1.0f;

        // Handle the movement
        if (grounded)
        {
            speed = Input.GetButton("Run") ? runSpeed : walkSpeed;

            moveDirection = new Vector3(inputX * inputModifyFactor, 0f, inputY * inputModifyFactor);
            moveDirection = myTransform.TransformDirection(moveDirection) * speed;

            // Jump!
            Jump(fuel, consumedFuel);
        }
        else
        {
            moveDirection.x = inputX * speed * inputModifyFactor;
            moveDirection.z = inputY * speed * inputModifyFactor;
            moveDirection = myTransform.TransformDirection(moveDirection);

            // Jump!
            Jump(fuel, consumedFuel);
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller, and set grounded true or false depending on whether we're standing on something
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

    [Serializable]
    public class PlayerState
    {
        public float InputX { get; private set; }

        public float InputY { get; private set; }

        public float ConsumedFuel { get; private set; }

        public float Fuel { get; private set; }

        public PlayerState(float inputX, float inputY, float consumedFuel, float fuel)
        {
            InputX = inputX;
            InputY = inputY;
            ConsumedFuel = consumedFuel;
            Fuel = fuel;
        }

        public bool IsEqual(PlayerState other)
        {
            return InputX.Equals(other.InputX) && InputY.Equals(other.InputY) && ConsumedFuel.Equals(other.ConsumedFuel);
        }
    }
}