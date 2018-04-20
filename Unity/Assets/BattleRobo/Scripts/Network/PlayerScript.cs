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
        /// The camera gameObject for the shooting.
        /// </summary>
        [SerializeField]
        private GameObject playerCamera;

        /// <summary>
        /// A cached version of the player photonview.
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
        /// The game over UI prefab.
        /// </summary>
        [SerializeField]
        private GameObject gameOverUI;

        /// <summary>
        /// The weapon holder script.
        /// </summary>
        [SerializeField]
        private WeaponHolderScript weaponHolder;

        /// <summary>
        /// Photon player ID.
        /// </summary>
        [HideInInspector]
        public int playerID;

        //movement variable
        private Vector3 moveDirection = Vector3.zero;
        private bool grounded;
        private float speed;

        //health variable
        public int maxHealth = 100;

        //fly variables
        private float fuelAmount = 1f;
        private float maxFuelAmount = 1f;
        private Vector3 fly;

        //tranform variables
        public float currentRot;
        private Transform myTransform;

        //weapon variables
        private WeaponScript activeWeapon;

        //safe zone variables
        public bool inStorm;
        private float waitingTime = 1f;
        private float timer;

        //water variables
        public bool inWater;

        // - player inventory
        private PlayerInventory playerInventory;

        //variable qui permet de tester en offline mode
        public bool isOfline;

        //Initialize server values for this player
        private void Awake()
        {
            PhotonNetwork.offlineMode = isOfline;
            //only let the master do initialization
            if (!PhotonNetwork.isMasterClient)
                return;

            //set players current health value after joining
            myPhotonView.SetHealth(maxHealth);
        }

        private void Start()
        {
            //set the player ID
            playerID = myPhotonView.ownerId;

            //player add itself to the dictionnary of alive player using his player ID
            GameManagerScript.GetInstance().alivePlayers.Add(playerID, gameObject);

            //called only for this client 
            if (!photonView.isMine)
                return;
            
            //instantiate the game over UI only for this client
            GameObject gOverUI = Instantiate(gameOverUI, Vector3.zero, Quaternion.identity);
            gOverUI.SetActive(false);
            GameManagerScript.GetInstance().gameOverUI = gOverUI;
            GameManagerScript.GetInstance().gameOverUiScript = gOverUI.GetComponent<GameOverUIScript>();

            //activate the player UI
            playerUI.SetActive(true);

            //activate camera only for this player
            playerCamera.SetActive(true);

            //set name in the UI
            uiScript.playerNameText.text = myPhotonView.GetName();

            //update health, shield and fuel
            uiScript.UpdateHealth(myPhotonView.GetHealth());
            uiScript.UpdateFuel(fuelAmount);
            uiScript.UpdateShield(myPhotonView.GetShield());

            uiScript.UpdateAliveText(GameManagerScript.GetInstance().alivePlayerNumber);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            myTransform = transform;
            speed = walkSpeed;

            thrusters.SetActive(false);

            // - initialise player inventory
            playerInventory = new PlayerInventory(playerCamera, uiScript, photonView);

            //set a global reference to the local player
            GameManagerScript.GetInstance().localPlayer = this;
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
            //uiScript.UpdateFuel(fuelAmount);
            uiScript.UpdateShield(myPhotonView.GetShield());
        }

        private void FixedUpdate()
        {
            if (!photonView.isMine)
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
            //set the active weapon to the current weapon
            activeWeapon = weaponHolder.currentWeapon;

            timer += Time.deltaTime;

            if (!photonView.isMine)
                return;

            //update the storm timer in the UI
            if (StormManagerScript.GetInstance().GetTimer() >= 0)
            {
                uiScript.UpdateStormTimer(StormManagerScript.GetInstance().GetTimer() + 1);
            }

            //apply damage to player in the storm
            if (inStorm)
            {
                if (timer > waitingTime)
                {
                    TakeDamage(StormManagerScript.GetInstance().stormDmg);
                    timer = 0f;
                }
            }

            //apply damage to player in the water
            if (inWater)
            {
                //insta death when the player touches the water
                TakeDamage(200);
            }

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

            uiScript.UpdateFuel(fuelAmount);

            fly = Vector3.zero;
            if (Input.GetButton("Jump") && fuelAmount > 0f)
            {
                fuelAmount -= fuelDecreaseSpeed * Time.deltaTime;
                //myPhotonView.SetFuel(fuelAmount);
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
                maxFuelAmount = fuelAmount;
            }

            if (Input.GetButtonDown("Fire1") && activeWeapon != null)
            {
                if (activeWeapon.CanFire())
                {
                    //send shot request with to server
                    myPhotonView.RPC("ShootRPC", PhotonTargets.AllViaServer);
                }
            }

            // - switch on active item
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerInventory.SwitchActiveIndex(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerInventory.SwitchActiveIndex(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                playerInventory.SwitchActiveIndex(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                playerInventory.SwitchActiveIndex(3);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                playerInventory.SwitchActiveIndex(4);
            }

            // Loot
            if (Input.GetKeyDown(KeyCode.F))
            {
                playerInventory.Collect();
            }
            
            // Loot
            if (Input.GetKeyDown(KeyCode.G))
            {
                playerInventory.Drop(myTransform.position);
            }

            fuelAmount = Mathf.Clamp(fuelAmount, 0f, 1f);
        }

        private void LateUpdate()
        {
            if (!photonView.isMine)
                return;

            // Rotate the player on the X axis
            currentRot -= Input.GetAxisRaw("Mouse Y") * aimSensitivity;
            currentRot = Mathf.Clamp(currentRot, -60f, 60f);

            // Make the weapon look in the same direction as the cam
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

        /// <summary>
        /// Server only: calculate damage to be taken by the Player,
        /// triggers kills increase and workflow on death.
        /// </summary>
        public void TakeDamage(int hitPoint, int killerID = -1)
        {
            //store network variables temporary
            int health = myPhotonView.GetHealth();
            int shield = myPhotonView.GetShield();

            //reduce shield on hit
            if (shield > 0)
            {
                myPhotonView.SetShield(shield - hitPoint);
                return;
            }

            //substract health by damage
            //locally for now, to only have one update later on
            health -= hitPoint;

            //the player is dead
            if (health <= 0)
            {
                //if we took damage from another player
                if (killerID != -1)
                {
                    //get killer and increase kills for that player
                    myPhotonView.RPC("UpdateKillsRPC", PhotonTargets.MasterClient, killerID);
                }

                //tell all clients that the player is dead
                myPhotonView.RPC("IsDeadRPC", PhotonTargets.All, playerID);
            }
            else
            {
                //we didn't die, set health to new value
                myPhotonView.SetHealth(health);
            }
        }

        public PlayerInventory GetInventory()
        {
            return playerInventory;
        }

        //called on all clients when the player is dead
        [PunRPC]
        private void IsDeadRPC(int id)
        {
            //out reference to the dead player
            GameObject player;
            //deactivate the dead player
            GameManagerScript.GetInstance().alivePlayers.TryGetValue(id, out player);
            player.SetActive(false);
            //remove the player from the alive players dictionnary and decrease the number of player alive
            GameManagerScript.GetInstance().alivePlayers.Remove(id);
            GameManagerScript.GetInstance().alivePlayerNumber--;
        }

        //called on the master client when a player kills the current player
        [PunRPC]
        private void UpdateKillsRPC(int id)
        {
            GameObject player;
            GameManagerScript.GetInstance().alivePlayers.TryGetValue(id, out player);

            if (player != null)
            {
                player.GetComponent<PlayerScript>().myPhotonView.AddKills();
            }
        }

        //called on the server first but forwarded to all clients
        [PunRPC]
        private void ShootRPC()
        {
            //fire the current weapon
            activeWeapon.Fire(playerCamera.transform, playerID);
        }

        [PunRPC]
        private void EquipWeaponRPC(int weaponIndex)
        {
            weaponHolder.EquipWeapon(weaponIndex);
        }

        [PunRPC]
        private void TakeObject(int lootTrackerId)
        {
            LootSpawnerScript.GetLootTracker()[lootTrackerId].GetComponent<PlayerObject>().Hide();           
        }

        [PunRPC]
        private void DropObject(int lootTrackerId, Vector3 position)
        {
            LootSpawnerScript.GetLootTracker()[lootTrackerId].GetComponent<PlayerObject>().Drop(position);
        }
    }
}