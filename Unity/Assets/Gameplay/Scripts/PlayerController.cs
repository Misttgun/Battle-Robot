using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Mouse Settings")] 
    [SerializeField] private float aimSensitivity = 5f;
    [SerializeField] private float aimSensitivityY = 10f;

    [Header("Movement Settings")] 
    [SerializeField] private float walkSpeed = 6.0f;
    [SerializeField] private float runSpeed = 11.0f;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float fuelDecreaseSpeed = 0.1f;
    [SerializeField] private float fuelRegenSpeed = 0.05f;
    [SerializeField] private float flyForce = 100f;

    [Header("Required Components")] 
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform roboChest;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject cam;

    private Vector3 moveDirection = Vector3.zero;
    private bool grounded;
    private float speed;

    private float fallStartLevel;
    private float fuelAmount = 1f;
    private Vector3 fly;

    private Vector3 roboRotY;
    private float roboRotUD;
    private float currentRot;

    private Transform myTransform;

    public float FuelAmount
    {
        get { return fuelAmount; }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
    }

    private void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        // Limit the diagonal speed
        float inputModifyFactor = Math.Abs(inputX) > 0.0001f && Math.Abs(inputY) > 0.0001f ? .7071f : 1.0f;

        // Handle the movement
        if (grounded)
        {
            speed = Input.GetButton("Run") ? runSpeed : walkSpeed;

            moveDirection = new Vector3(inputX * inputModifyFactor, 0f, inputY * inputModifyFactor);
            moveDirection = myTransform.TransformDirection(moveDirection) * speed;

            // Jump!
            Jump();
        }
        else
        {
            moveDirection.x = inputX * speed * inputModifyFactor;
            moveDirection.z = inputY * speed * inputModifyFactor;
            moveDirection = myTransform.TransformDirection(moveDirection);

            // Jump!
            Jump();
        }

        // Rotate the player on the Y axis
        myTransform.rotation *= Quaternion.Euler(0, Input.GetAxisRaw("Mouse X") * aimSensitivityY, 0);

        // Rotate the player on the X axis
        currentRot -= Input.GetAxisRaw("Mouse Y") * aimSensitivity;
        currentRot = Mathf.Clamp(currentRot, -60f, 60f);
        roboChest.transform.localEulerAngles = new Vector3(0f, 0f, -currentRot);

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller, and set grounded true or false depending on whether we're standing on something
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
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

        fly = Vector3.zero;
        if (Input.GetButton("Jump") && fuelAmount > 0f)
        {
            fuelAmount -= fuelDecreaseSpeed * Time.deltaTime;
            if (fuelAmount >= 0.04f)
            {
                fly = Vector3.up * flyForce;
            }
        }
        else
        {
            fuelAmount += fuelRegenSpeed * Time.deltaTime;
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
}