using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon;
using UnityEngine;
using UnityEngine.Rendering;

namespace BattleRobo
{
    public class RoboLogicScript : PunBehaviour
    {
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
        /// The player animator.
        /// </summary>
        public Animator animator;

        /// <summary>
        /// The thrusters gameobject for the flying effect.
        /// </summary>
        public GameObject thrusters;

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
        private PlayerLevelStreamerScript playerLevelStreamer;

        /// <summary>
        /// The player audiosource.
        /// </summary>
        public AudioSource audioSource;

        /// <summary>
        /// Array for storing player audio clips.
        /// </summary>
        public AudioClip[] audioClips;

        [SerializeField]
        private SkinnedMeshRenderer[] playerSkinedMesh;

        /// <summary>
        /// Photon player ID.
        /// </summary>
        [HideInInspector]
        public int playerID;

        //health variable
        public int maxHealth = 100;

        //fly variables
        public float fuelAmount = 1f;
        public float MaxFuelAmount = 1f;

        //audio variables
        private bool playAudio;
        public bool isJumpingAudio;
        public bool isMoovingAudio;

        //safe zone variables
        public bool inStorm;
        private const float waitingTime = 1f;
        private float timer;

        //water variables
        public bool inWater;

        //player inventory
        private PlayerInventory playerInventory;

        //ui variables
        private int previousHealth;
        private int previousKills;
        private int previousShield;
        private int previousAliveNumber;

        // game state
        public bool isInPause;

        //Initialize server values for this player
        private void Awake()
        {
            //set the player ID
            playerID = photonView.ownerId;

            //initialise player inventory
            playerInventory = new PlayerInventory(playerCameraTransform, uiScript, photonView, weaponHolder);

            //only let the master do initialization
            if (!PhotonNetwork.isMasterClient)
                return;

            //set players current health value after joining
            photonView.SetHealth(maxHealth);
        }

        private void Start()
        {
            //activate all the level streamer on every client
            playerLevelStreamer.gameObject.SetActive(true);
            playerLevelStreamer.target = transform;

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
            playerCamera.enabled = true;
            playerCameraAudio.enabled = true;

            //set name in the UI
            uiScript.playerNameText.text = photonView.GetName();

            //update health, shield and fuel
            uiScript.UpdateHealth(photonView.GetHealth());
            uiScript.UpdateFuel(fuelAmount);
            uiScript.UpdateShield(photonView.GetShield());
            uiScript.UpdateKillsText(photonView.GetKills());

            uiScript.UpdateAliveText(GameManagerScript.alivePlayerNumber);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            thrusters.SetActive(false);

            //set a global reference to the local player
            GameManagerScript.GetInstance().localPlayer = this;

            photonView.RPC("SendDBTokenRPC", PhotonTargets.MasterClient, playerID, PlayerInfoScript.GetInstance().GetDBToken());

            for (int i = 0; i < playerSkinedMesh.Length; i++)
            {
                playerSkinedMesh[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }

        public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
        {
            //only react on property changes for this player
            PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;

            if (player != null && !player.Equals(photonView.owner))
                return;

            //update values that could change any time for visualization to stay up to date
            uiScript.UpdateHealth(photonView.GetHealth());
            uiScript.UpdateShield(photonView.GetShield());
            uiScript.UpdateKillsText(photonView.GetKills());
        }

        private void Update()
        {
            isInPause = GameManagerScript.GetInstance().IsGamePause();

            if (isInPause)
            {
                //stop sound when in pause
                if (audioSource.isPlaying)
                    audioSource.Stop();

                return;
            }

            //take storm and water damage only on the master client
            if (PhotonNetwork.isMasterClient)
            {
                timer += Time.deltaTime;

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

            if (!photonView.isMine)
                return;

            //constantly update fuel as it change constantly
            uiScript.UpdateFuel(fuelAmount);

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
        }

        /// <summary>
        /// Server only: calculate damage to be taken by the Player,
        /// triggers kills increase and workflow on death.
        /// </summary>
        public void TakeDamage(int hitPoint, int killerID = -1)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            //store network variables temporary
            int health = photonView.GetHealth();
            int shield = photonView.GetShield();

            //reduce shield on hit
            if (shield > 0 && killerID != -1)
            {
                photonView.SetShield(shield - hitPoint);
                return;
            }

            //if player is already dead, but we can still shoot him... don't ask me why
            if (health <= 0)
            {
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
                    photonView.RPC("UpdateKillsRPC", PhotonTargets.MasterClient, killerID);
                }

                //set the player current rank
                photonView.SetRank(GameManagerScript.alivePlayerNumber);

                // set dead player stats
                SetPlayerStats(photonView.GetKills(), 0, GameManagerScript.GetInstance().dbTokens[playerID]);


                //tell all clients that the player is dead
                photonView.RPC("IsDeadRPC", PhotonTargets.All, playerID);

                //decrease the number of player alive
                GameManagerScript.alivePlayerNumber--;
            }
            else
            {
                //we didn't die, set health to new value
                photonView.SetHealth(health);
            }
        }

        private void SetPlayerStats(int kills, int win, string token)
        {
            string url = "http://51.38.235.234:8080/update_player?token=" + token + "&kill=" + kills + "&win=" + win;

            // don't wait for response
            WWW www = new WWW(url);
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

            //remove the player from the alive players dictionnary
            GameManagerScript.GetInstance().alivePlayers.Remove(id);

            if (GameManagerScript.GetInstance().localPlayer == this)
            {
                //set the local values for the gameover screen
                GameManagerScript.GetInstance().hasLost = true;
            }
            else
            {
                if (GameManagerScript.GetInstance().alivePlayers.Count == 1)
                {
                    int player_id = GameManagerScript.GetInstance().alivePlayers.Keys.First();

                    photonView.RPC("WinnerRPC", PhotonTargets.MasterClient, player_id);
                }
            }
        }

        //called on the master client when a player kills the current player
        [PunRPC]
        private void UpdateKillsRPC(int id)
        {
            GameObject player;
            GameManagerScript.GetInstance().alivePlayers.TryGetValue(id, out player);

            if (player != null)
            {
                player.GetComponent<PhotonView>().AddKills();
            }
        }

        //called on the master client only
        [PunRPC]
        private void ShootRPC(int shooterId)
        {
            if (weaponHolder.currentWeapon != null)
            {
                //temp value in case we quickly change weapon after we shoot
                var weapon = weaponHolder.currentWeapon;

                weapon.Fire(playerCameraTransform, playerID);

                //play the weapon sound
                AudioManagerScript.Play3D(audioSource, weapon.weaponSound);

                //the item can have changed since the player shoot. Update UI only if necessary
                var update = weapon == weaponHolder.currentWeapon;

                if (playerID == shooterId && update)
                    uiScript.SetAmmoCounter(weapon.currentAmmo, weapon.GetMagazineSize());
            }
        }

        [PunRPC]
        private void EquipWeaponRPC(int weaponIndex, float currentAmmo)
        {
            if (weaponIndex == -1)
            {
                //return the layer animation to normal
                animator.SetLayerWeight(2, 0);
            }

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
                weaponHolder.SetWeapon(weapon, weapon.currentAmmo);
                uiScript.SetAmmoCounter(weapon.currentAmmo, weapon.GetMagazineSize());
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

            //return the layer animation to normal
            animator.SetLayerWeight(2, 0);
        }

        [PunRPC]
        private void SetPause()
        {
            var myPlayerScript = GameManagerScript.GetInstance().localPlayer;

            // - Set Pause UI
            // myPlayerScript.playerUI.GetComponent<PlayerUIScript>().EnablePause(true);
            GameManagerScript.GetInstance().SetPause(true);
        }

        [PunRPC]
        private void CancelPause()
        {
            var myPlayerScript = GameManagerScript.GetInstance().localPlayer;

            // - Set Pause UI
            //myPlayerScript.playerUI.GetComponent<PlayerUIScript>().EnablePause(false);
            GameManagerScript.GetInstance().SetPause(false);
        }

        [PunRPC]
        private void DamageIndicatorRPC(Vector3 shooterPos)
        {
            uiScript.UpdateDamageIndicator(shooterPos);
        }

        [PunRPC]
        private void HitMarkerRPC()
        {
            uiScript.UpdateHitMarker();
        }

        [PunRPC]
        private void SendDBTokenRPC(int id, string token)
        {
            // only the master client should save the db token
            if (PhotonNetwork.isMasterClient)
                GameManagerScript.GetInstance().dbTokens.Add(id, token);
        }


        [PunRPC]
        private void WinnerRPC(int id)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            //check if there is only one player
            if (GameManagerScript.GetInstance().alivePlayers.Count == 1)
                SetPlayerStats(photonView.GetKills(), 1, GameManagerScript.GetInstance().dbTokens[id]);
        }
        
        //TODO Finir la logique du joueur avec les appels de RPC. Commencer à optimiser le code.
    }
}