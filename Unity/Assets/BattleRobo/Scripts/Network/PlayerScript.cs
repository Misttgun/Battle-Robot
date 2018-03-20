using UnityEngine;
using System;
using Photon;
using Debug = System.Diagnostics.Debug;

namespace BattleRobo
{
    /// <summary>
    /// Networked player class implementing movement control and shooting.
    /// Contains both server and client logic in an authoritative approach.
    /// </summary> 
    public class PlayerScript : PunBehaviour
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
        /// A cached vrsion of the player photonview.
        /// </summary>
        [SerializeField]
        private PhotonView myPhotonView;

        /// <summary>
        /// The player animator.
        /// </summary>
        [SerializeField]
        private Animator animator;

        /// <summary>
        /// The thrusters gameobject for the flying effect.
        /// </summary>
        [SerializeField]
        private GameObject thrusters;

        /// <summary>
        /// The in game UI script.
        /// </summary>
        [SerializeField]
        private PlayerUIScript uiScript;

        /// <summary>
        /// The in game UI script.
        /// </summary>
        [SerializeField]
        private GameObject playerUI;

        /// <summary>
        /// Last player gameobject that killed this one.
        /// </summary>
        [HideInInspector]
        public GameObject killedBy;

        private Vector3 moveDirection = Vector3.zero;
        private bool grounded;
        private float speed;
        public int maxHealth = 100;
        private float fuelAmount = 1f;
        private float maxFuelAmount = 1f;
        private Vector3 fly;
        private float currentRot;
        private Transform myTransform;


        //Initialize server values for this player
        private void Awake()
        {
            //only let the master do initialization
            if (!PhotonNetwork.isMasterClient)
                return;

            //set players current health value after joining
            myPhotonView.SetHealth(maxHealth);
        }


        /// <summary>
        /// Initialize synced values on every client.
        /// Initialize camera and input for this local client.
        /// </summary>
        private void Start()
        {
			//player add itself to the list of alive player
			GameManagerScript.GetInstance().alivePlayers.Add(gameObject);

            //called only for this client 
            if (!photonView.isMine)
                return;
            
            //activate the player UI
            playerUI.SetActive(true);
            
            //set name in the UI
            uiScript.playerNameText.text = myPhotonView.GetName();
            
            //update health, shield and fuel
            uiScript.UpdateHealth(myPhotonView.GetHealth());
            uiScript.UpdateFuel(myPhotonView.GetFuel());
            uiScript.UpdateShield(myPhotonView.GetShield());
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            myTransform = transform;
            speed = walkSpeed;

            thrusters.SetActive(false);

            //set a global reference to the local player
            //GameManager.GetInstance().localPlayer = this;
        }


        /// <summary>
        /// This method gets called whenever player properties have been changed on the network.
        /// </summary>
        public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
        {
            //only react on property changes for this player
            PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
            
            Debug.Assert(player != null, "player != null");
            if (!player.Equals(photonView.owner))
                return;

            //update values that could change any time for visualization to stay up to date
            uiScript.UpdateHealth(myPhotonView.GetHealth());
            uiScript.UpdateFuel(myPhotonView.GetFuel());
            uiScript.UpdateShield(myPhotonView.GetShield());
        }
        
        private void FixedUpdate()
        {
            if(!photonView.isMine)
                return;
            
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
                
                // Disable the thrusters when the player is not flying
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
            if(!photonView.isMine)
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

            fly = Vector3.zero;
            if (Input.GetButton("Jump") && fuelAmount > 0f)
            {
                fuelAmount -= fuelDecreaseSpeed * Time.deltaTime;
                myPhotonView.SetFuel(fuelAmount);
                var consumedFuel = maxFuelAmount - fuelAmount;
                
                if (fuelAmount >= 0.1f)
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

            fuelAmount = Mathf.Clamp(fuelAmount, 0f, 1f);
        }

        private void LateUpdate()
        {
            // Rotate the player on the X axis
            currentRot -= Input.GetAxisRaw("Mouse Y") * aimSensitivity;
            currentRot = Mathf.Clamp(currentRot, -60f, 60f);
            
            // Make the weapon loot in the same direction as the cam
            animator.SetFloat("AimAngle", -currentRot);
            roboHead.transform.localEulerAngles = new Vector3(0f, -currentRot, 0f);
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