using UnityEngine;
using System;
using Debug = System.Diagnostics.Debug;

namespace BattleRobo
{
    /// <summary>
    /// Networked player class implementing movement control and shooting.
    /// Contains both server and client logic in an authoritative approach.
    /// </summary> 
    public class PlayerScript : MonoBehaviour
    {
        /// <summary>
        /// Aim sensitivity.
        /// </summary>
        [Header("Mouse Settings")]
        [SerializeField]
        private float aimSensitivity = 5f;

        /// <summary>
        /// Player walk speed.
        /// </summary>
        [Header("Movement Settings")]
        [SerializeField]
        private float walkSpeed = 6.0f;

        /// <summary>
        /// Player run speed.
        /// </summary>
        [SerializeField]
        private float runSpeed = 11.0f;

        /// <summary>
        /// Gravity applied to the player.
        /// </summary>
        [SerializeField]
        private float gravity = 20.0f;

        /// <summary>
        /// Speed at which the fuel decrease.
        /// </summary>
        [SerializeField]
        private float fuelDecreaseSpeed = 0.1f;

        /// <summary>
        /// Speed at which the fuel regenerate.
        /// </summary>
        [SerializeField]
        private float fuelRegenSpeed = 0.05f;

        /// <summary>
        /// Fly force for the jetpack.
        /// </summary>
        [SerializeField]
        private float flyForce = 100f;

        /// <summary>
        /// the charactercontroller for the player.
        /// </summary>
        [Header("Required Components")]
        [SerializeField]
        private CharacterController controller;

        /// <summary>
        /// The robo head transform for the camera rotation.
        /// </summary>
        [SerializeField]
        private Transform roboHead;

        /// <summary>
        /// The camera gameObject for the shooting.
        /// </summary>
        [SerializeField]
        private GameObject playerCamera;

        //movement variable
        private Vector3 moveDirection = Vector3.zero;
        private bool grounded;
        private float speed;

        //fly variables
        private float fuelAmount = 1f;
        private float maxFuelAmount = 1f;
        private Vector3 fly;

        //tranform variables
        public float currentRot;
        private Transform myTransform;

        private void Start()
        {

            //activate camera only for this player
            playerCamera.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

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
                //myPhotonView.SetFuel(fuelAmount);
                var consumedFuel = maxFuelAmount - fuelAmount;

                if (fuelAmount >= 0.1f)
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

        private void LateUpdate()
        {
            // Rotate the player on the X axis
            currentRot -= Input.GetAxisRaw("Mouse Y") * aimSensitivity;
            currentRot = Mathf.Clamp(currentRot, -60f, 60f);

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