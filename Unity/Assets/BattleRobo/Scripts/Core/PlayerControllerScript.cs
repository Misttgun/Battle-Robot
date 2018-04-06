using System;
using UnityEngine;
using BattleRobo.Networking;

namespace BattleRobo
{
    public class PlayerControllerScript : Photon.PunBehaviour
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

        [SerializeField]
        private Transform roboHead;

        [SerializeField]
        private PhotonView myPhotonView;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private GameObject thrusters;

        [SerializeField]
        private PlayerInventory playerInventory;

        private Vector3 moveDirection = Vector3.zero;
        private bool grounded;
        private float speed;

        private float fallStartLevel;
        public float fuelAmount = 1f;
        private float maxFuelAmount = 1f;
        private Vector3 fly;
        public float currentRot;

        private Transform myTransform;

        public float FuelAmount
        {
            get { return fuelAmount; }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            myTransform = transform;
            speed = walkSpeed;

            thrusters.SetActive(false);
            playerInventory = new PlayerInventory();
        }


        public PlayerInventory GetInventory()
        {
            return playerInventory;
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
                // Disable the jump layer when the player is on the ground
                animator.SetLayerWeight(1, 0);
                
                // Disable the thrusters when the player is not flying
                thrusters.SetActive(false);
                
                speed = Input.GetButton("Run") ? runSpeed : walkSpeed;

                moveDirection = new Vector3(inputX * inputModifyFactor, 0f, inputY * inputModifyFactor);
                
                // Animate the player for the ground animation
                animator.SetFloat("VelX", moveDirection.x * speed);
                animator.SetFloat("VelY", moveDirection.z * speed);
                
                moveDirection = myTransform.TransformDirection(moveDirection) * speed;

                // Jump!
                Jump();
            }
            else
            {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                
                // Animate the player for the ground animation
                animator.SetFloat("VelX", moveDirection.x);
                animator.SetFloat("VelY", moveDirection.z);
                
                // Activate the thrusters
                thrusters.SetActive(true);
                
                moveDirection = myTransform.TransformDirection(moveDirection);

                // Jump!
                Jump();
            }

            // Rotate the player on the Y axis
            myTransform.rotation *= Quaternion.Euler(0, Input.GetAxisRaw("Mouse X") * aimSensitivity, 0);

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
                var consumedFuel = maxFuelAmount - fuelAmount;
                
                if (fuelAmount >= 0.15f)
                {
                    // We override the base layer when the player is jumping
                    animator.SetLayerWeight(1, 1);
                    
                    fly = Vector3.up * flyForce * consumedFuel;
                }
            }
            else
            {
                fuelAmount += fuelRegenSpeed * Time.deltaTime;
            }
            
            // Loot
            if (Input.GetKeyDown(KeyCode.F))
            {
                playerInventory.Collect();
            }
            
            // Drop
            if (Input.GetKeyDown(KeyCode.G))
            {
                var dropPosition = myTransform.position;
                dropPosition.y = 2;
                playerInventory.Drop(dropPosition);
            }

            fuelAmount = Mathf.Clamp(fuelAmount, 0f, 1f);
        }

        private void LateUpdate()
        {
            // Rotate the player on the X axis
            currentRot -= Input.GetAxisRaw("Mouse Y") * aimSensitivity;
            currentRot = Mathf.Clamp(currentRot, -60f, 60f);
            
            // Make the weapon loot in the same direction as the cam
            animator.SetFloat("AimAngle", currentRot);
            roboHead.transform.localEulerAngles = new Vector3(currentRot, 0f, 0f);
        }

        private void Jump()
        {
            if (fly != Vector3.zero)
            {
                moveDirection.y = fly.y;
            }
        }
    }
}