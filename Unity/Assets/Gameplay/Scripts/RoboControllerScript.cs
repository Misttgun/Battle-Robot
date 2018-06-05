using UnityEngine;
using System;
using System.Security.Cryptography.X509Certificates;
using Photon;

namespace BattleRobo
{
    /// <summary>
    /// Networked player class implementing movement control and shooting.
    /// Contains both server and client logic in an authoritative approach.
    /// </summary> 
    public class RoboControllerScript : PunBehaviour, IPunObservable
    {
        /// <summary>
        /// Aim sensitivity.
        /// </summary>
        [Header("Mouse Settings")]
        [SerializeField]
        private float aimSensitivity = 5f;

        /// <summary>
        /// Player run speed.
        /// </summary>[Header("Movement Settings")]
        [SerializeField]
        private float speed = 9.0f;

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

        public PlayerState playerState = new PlayerState();

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
        /// The camera transform for the shooting.
        /// </summary>
        [SerializeField]
        private Transform playerCameraTransform;

        /// <summary>
        /// The camera component.
        /// </summary>
        [SerializeField]
        private Camera playerCamera;

        /// <summary>
        /// The camera audio listener.
        /// </summary>
        [SerializeField]
        private AudioListener playerCameraAudio;

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
        /// The collider box for the player level streamer.
        /// </summary>
        [SerializeField]
        private GameObject playerLevelStreamer;

        /// <summary>
        /// The player audiosource.
        /// </summary>
        [SerializeField]
        private AudioSource audioSource;

        /// <summary>
        /// Array for storing player audio clips.
        /// </summary>
        [SerializeField]
        private AudioClip[] audioClips;

        /// <summary>
        /// Photon player ID.
        /// </summary>
        [HideInInspector]
        public int playerID;

        /// <summary>
        /// The Player Stats.
        /// </summary>
        [HideInInspector]
        public PlayerStats playerStats = new PlayerStats();

        //movement variable
        private Vector3 moveDirection = Vector3.zero;
        private bool grounded;

        //health variable
        public int maxHealth = 100;

        //fly variables
        public float fuelAmount = 1f;
        private float maxFuelAmount = 1f;
        private Vector3 fly;

        //tranform variables
        public float currentRot;
        private float networkCurrentRot;
        private Transform myTransform;
        private bool isMooving;

        //audio variables
        private bool playAudio;

        //safe zone variables
        public bool inStorm;
        private const float waitingTime = 1f;
        private float timer;

        //water variables
        public bool inWater;

        //player inventory
        private PlayerInventory playerInventory;

        //Initialize server values for this player
        private void Awake()
        {
            //set the player ID
            playerID = myPhotonView.viewID - 1;

            //initialise player inventory
            playerInventory = new PlayerInventory(playerCameraTransform, uiScript, myPhotonView, weaponHolder);

            //set players current health value after joining
            playerStats.Health = maxHealth;
        }

        private void Start()
        {
            //activate all the level streamer on the master client
            if (PhotonNetwork.isMasterClient)
            {
                playerLevelStreamer.SetActive(true);
            }

            myTransform = transform;

            //player add itself to the dictionnary of alive player using his player ID
            GameManagerScript.GetInstance().alivePlayers.Add(playerID, gameObject);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(currentRot);
                stream.SendNext(playerStats.Health);
                stream.SendNext(playerStats.Shield);
                stream.SendNext(playerStats.Kills);
                stream.SendNext(fuelAmount);
                stream.SendNext(isMooving);
            }
            else
            {
                networkCurrentRot = (float) stream.ReceiveNext();
                playerStats.Health = (int) stream.ReceiveNext();
                playerStats.Shield = (int) stream.ReceiveNext();
                playerStats.Kills = (int) stream.ReceiveNext();
                fuelAmount = (float) stream.ReceiveNext();
                isMooving = (bool) stream.ReceiveNext();
            }
        }

        private void FixedUpdate()
        {
            if (GameManagerScript.GetInstance().IsGamePause())
                return;

            // Limit the diagonal speed
            float inputModifyFactor = Math.Abs(playerState.inputX) > 0.0001f && Math.Abs(playerState.inputY) > 0.0001f ? .7071f : 1.0f;

            // Handle the movement
            if (grounded)
            {
                // Disable the jump layer when the player is on the ground
                animator.SetLayerWeight(1, 0);

                // Disable the thrusters when the player is not flying
                thrusters.SetActive(false);

                moveDirection = new Vector3(playerState.inputX * inputModifyFactor, 0f, playerState.inputY * inputModifyFactor);

                //Stop the jetpack sound if we are grounded
                if (audioSource.isPlaying && audioSource.clip == audioClips[1])
                {
                    audioSource.Stop();
                }

                //Play the walk/run audio
                if (isMooving && !audioSource.isPlaying)
                {
                    AudioManagerScript.Play3D(audioSource, audioClips[0], 1f);
                }

                // Animate the player for the ground animation
                animator.SetFloat("VelX", moveDirection.x * speed);
                animator.SetFloat("VelY", moveDirection.z * speed);

                moveDirection = myTransform.TransformDirection(moveDirection) * speed;

                // Jump!
                Jump();
            }
            else
            {
                moveDirection.x = playerState.inputX * speed * inputModifyFactor;
                moveDirection.z = playerState.inputY * speed * inputModifyFactor;

                // Animate the player for the ground animation
                animator.SetFloat("VelX", moveDirection.x);
                animator.SetFloat("VelY", moveDirection.z);

                // Disable the thrusters when the player is not flying
                thrusters.SetActive(true);

                //Stop the walking/running sound if we are not grounded
                if (audioSource.isPlaying && audioSource.clip == audioClips[0])
                {
                    audioSource.Stop();
                }

                //Play the jetpack sound
                if (!audioSource.isPlaying)
                {
                    AudioManagerScript.Play3D(audioSource, audioClips[1], 1);
                }

                moveDirection = myTransform.TransformDirection(moveDirection);

                // Jump!
                Jump();
            }

            // Rotate the player on the Y axis
            myTransform.rotation *= Quaternion.Euler(0, playerState.mouseInput.x * aimSensitivity, 0);

            // Apply gravity
            moveDirection.y -= gravity * Time.deltaTime;

            //Set the mooving bool
            isMooving = Math.Abs(moveDirection.x) > 0.0001f || Math.Abs(moveDirection.z) > 0.0001f;

            // Move the controller, and set grounded true or false depending on whether we're standing on something
            grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        }

        private void Update()
        {
            uiScript.UpdateFuel(fuelAmount);
            uiScript.UpdateHealth(playerStats.Health);
            uiScript.UpdateShield(playerStats.Shield);

            if (GameManagerScript.GetInstance().IsGamePause())
                return;

            timer += Time.deltaTime;

            uiScript.UpdateAliveText(GameManagerScript.GetInstance().alivePlayerNumber);

            //update the storm timer in the UI
//            if (StormManagerScript.GetInstance().GetStormTimer() >= 0)
//            {
//                uiScript.UpdateStormTimer(StormManagerScript.GetInstance().GetStormTimer() + 1);
//            }

            if (!PhotonNetwork.isMasterClient)
            {
                UpdateNetworkHeadRotation();
            }

            if (!PhotonNetwork.isMasterClient)
                return;

            fly = Vector3.zero;
            if (playerState.isJumping && fuelAmount > 0f)
            {
                fuelAmount -= fuelDecreaseSpeed * Time.deltaTime;
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

            fuelAmount = Mathf.Clamp(fuelAmount, 0f, 1f);

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
                TakeDamage(300);
            }
        }

        private void LateUpdate()
        {
            // Rotate the player on the X axis
            currentRot -= playerState.mouseInput.y * aimSensitivity;
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
            int health = playerStats.Health;
            int shield = playerStats.Shield;

            //reduce shield on hit
            if (shield > 0)
            {
                playerStats.Shield = shield - hitPoint;
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
                playerStats.Health = health;
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
            var found = GameManagerScript.GetInstance().alivePlayers.TryGetValue(id, out player);

            if (found)
            {
                player.SetActive(false);
            }

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
                player.GetComponent<RoboControllerScript>().playerStats.AddKills();
            }
        }

        //called on the server first but forwarded to all clients
        [PunRPC]
        private void ShootRPC(int shooterId, int itemId)
        {
            //fire the right weapon
            var currentItem = LootSpawnerScript.GetLootTracker()[itemId].GetComponent<PlayerObjectScript>();
            WeaponScript weapon = null;


            if (currentItem)
                weapon = currentItem.GetWeapon();

            if (weapon != null)
            {
                weapon.Fire(playerCameraTransform, playerID);

                //play the weapon sound
                AudioManagerScript.Play3D(audioSource, weapon.weaponSound, 1f);

                // the iteam can have changed since the player shoot. Update UI only if necessary
                var update = playerInventory.getCurrentActive().GetLootTrackerIndex() == itemId;
                if (playerID == shooterId && update)
                    uiScript.SetAmmoCounter(weapon.GetCurrentAmmo(), weapon.GetMagazineSize());
            }
        }

        [PunRPC]
        private void EquipWeaponRPC(int weaponIndex, float currentAmmo)
        {
            weaponHolder.EquipWeapon(weaponIndex, currentAmmo);
        }

        [PunRPC]
        private void TakeObject(int lootTrackerId, int slotIndex, int senderId)
        {
            var playerObject = LootSpawnerScript.GetLootTracker()[lootTrackerId].GetComponent<PlayerObjectScript>();

            playerInventory.AddObject(playerObject, slotIndex);
            uiScript.SetItemUISlot(playerObject, slotIndex);

            // equip weapon if the index is already selected
            if (slotIndex == playerInventory.currentSlotIndex)
            {
                var weapon = playerObject.GetComponent<WeaponScript>();
                weaponHolder.SetWeapon(weapon, weapon.GetCurrentAmmo());
                uiScript.SetAmmoCounter(weapon.GetCurrentAmmo(), weapon.GetMagazineSize());
            }

            if (playerObject.IsAvailable())
            {
                playerObject.SetAvailable(false);
                playerObject.Hide();
            }

            // - several player tried to loot the object at the same time, 
            // only the first one should see it in its inventory
            else
            {
                if (playerID == senderId)
                    playerInventory.CancelCollect(lootTrackerId);
            }

            // - Object is no longer in looting mode
            playerObject.SetLooting(false);
        }

        [PunRPC]
        private void UpdateWeapon(int lootTrackerId, float ammoCount)
        {
            LootSpawnerScript.GetLootTracker()[lootTrackerId].GetComponent<WeaponScript>().SetCurrentAmmo(ammoCount);
        }

        [PunRPC]
        private void DropObject(int lootTrackerId, Vector3 position)
        {
            var playerObject = LootSpawnerScript.GetLootTracker()[lootTrackerId].GetComponent<PlayerObjectScript>();

            playerObject.Drop(position);

            // - remove object from player inventory
            playerInventory.inventory[playerInventory.currentSlotIndex].Drop();

            // - update UI
            uiScript.SetItemUISlot(null, playerInventory.currentSlotIndex);
            uiScript.SetAmmoCounter(-1f, -1f);

            // - unequip weapon
            weaponHolder.SetWeapon(null, 0f);
        }

        [PunRPC]
        private void SetPause()
        {
            var myPlayerScript = GameManagerScript.GetInstance().localPlayer;

            // - Set Pause UI
            myPlayerScript.playerUI.GetComponent<PlayerUIScript>().EnablePause(true);
            GameManagerScript.GetInstance().SetPause(true);
        }

        [PunRPC]
        private void CancelPause()
        {
            var myPlayerScript = GameManagerScript.GetInstance().localPlayer;

            // - Set Pause UI
            myPlayerScript.playerUI.GetComponent<PlayerUIScript>().EnablePause(false);
            GameManagerScript.GetInstance().SetPause(false);
        }

        public void ClientMovement(float inputX, float inputY, bool isJumping, Vector2 mouseInput)
        {
            playerState = new PlayerState(inputX, inputY, isJumping, mouseInput);
        }

        public void ClientPause()
        {
            myPhotonView.RPC(!GameManagerScript.GetInstance().IsGamePause() ? "SetPause" : "CancelPause", PhotonTargets.AllViaServer);
        }

        public void ClientShooting()
        {
            if (playerInventory.getCurrentActive() != null)
            {
                var weapon = playerInventory.getCurrentActive().GetWeapon();

                if (weapon && weapon.CanFire())
                {
                    var itemId = playerInventory.getCurrentActive().GetLootTrackerIndex();

                    //send shot request to server. We must pass the current inventoryIndex because, if 
                    // the player switch very quickly after the, shot, the wrong weapon is used
                    myPhotonView.RPC("ShootRPC", PhotonTargets.AllViaServer, playerID, itemId);
                }
            }
        }

        public void ClientLoot()
        {
            playerInventory.Collect();
        }

        public void ClientDrop()
        {
            //on drop l'objet un peu plus haut que la position y du joueur sinon, l'objet rentre dans le sol et n'est plus ramassable
            var newPositipn = myTransform.position + new Vector3(0f, 0.1f, 0f);
            playerInventory.Drop(newPositipn);
        }

        public void ClientSwitchWeapon(int index)
        {
            playerInventory.SwitchActiveIndex(index);
        }

        public void SetUp()
        {
            //instantiate the game over UI only for this client
            GameObject gOverUI = Instantiate(gameOverUI, Vector3.zero, Quaternion.identity);
            gOverUI.SetActive(false);
            GameManagerScript.GetInstance().gameOverUI = gOverUI;
            GameManagerScript.GetInstance().gameOverUiScript = gOverUI.GetComponent<GameOverUIScript>();

            //activate the player UI
            playerUI.SetActive(true);

            //activate camera only for this player
            playerCamera.enabled = true;
            playerCameraAudio.enabled = true;

            //update health, shield and fuel
            uiScript.UpdateHealth(playerStats.Health);
            uiScript.UpdateFuel(fuelAmount);
            uiScript.UpdateShield(playerStats.Shield);

            uiScript.UpdateAliveText(GameManagerScript.GetInstance().alivePlayerNumber);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            thrusters.SetActive(false);

            if (!playerLevelStreamer.activeSelf)
            {
                playerLevelStreamer.SetActive(true);
            }

            //set a global reference to the local player
            GameManagerScript.GetInstance().localPlayer = this;

            //set name in the UI
            uiScript.playerNameText.text = PhotonNetwork.player.NickName;
        }

        private void UpdateNetworkHeadRotation()
        {
            currentRot = Mathf.MoveTowards(currentRot, networkCurrentRot, aimSensitivity);
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
                return inputX.Equals(other.inputX) && inputY.Equals(other.inputY) && isJumping.Equals(other.isJumping) && mouseInput == other.mouseInput;
            }
        }

        [Serializable]
        public class PlayerStats
        {
            private int health;
            private int shield;
            private int kills;

            public PlayerStats()
            {
                health = 0;
                shield = 0;
                kills = 0;
            }

            public int Health
            {
                get { return health; }
                set { health = value; }
            }

            public int Shield
            {
                get { return shield; }
                set { shield = value; }
            }

            public int Kills
            {
                get { return kills; }
                set { kills = value; }
            }

            public void AddKills()
            {
                kills++;
            }
        }
    }
}