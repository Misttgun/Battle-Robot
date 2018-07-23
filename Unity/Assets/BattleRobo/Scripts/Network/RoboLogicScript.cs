using System;
using System.Collections;
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
        /// The weapon holder script.
        /// </summary>
        [SerializeField]
        private WeaponHolderScript weaponHolder;

        /// <summary>
        /// The weapon holder script.
        /// </summary>
        [SerializeField]
        private ConsommableHolderScript consommableHolder;

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
        [HideInInspector]
        public int maxHealth = 100;

        //audio variables
        private bool playAudio;

        [HideInInspector]
        public bool isJumpingAudio;

        [HideInInspector]
        public bool isMoovingAudio;

        //player inventory
        private PlayerInventory playerInventory;

        private int index;
        private int currentIndex = -1;

        //ui variables
        private int previousAliveNumber;

        // game state
        [HideInInspector]
        public bool isInPause;

        private WaitForSeconds waitOneSec = new WaitForSeconds(1);

        //Initialize server values for this player
        private void Awake()
        {
            //set the player ID
            playerID = photonView.ownerId;

            //initialise player inventory
            playerInventory = new PlayerInventory(playerCameraTransform, uiScript, photonView, weaponHolder, consommableHolder);

            //only let the master do initialization
            if (!PhotonNetwork.isMasterClient)
                return;

            //set players current health value after joining
            photonView.SetHealth(maxHealth);
            photonView.SetShield(0);
            photonView.SetKills(0);
        }

        private void Start()
        {
            //player add itself to the dictionnary of alive player using his player ID
            GameManagerScript.GetInstance().alivePlayers.Add(playerID, gameObject);

            //intialization of the pause counter for all players
            GameManagerScript.GetInstance().pauseCounter.Add(playerID, 3);

            //called only for this client 
            if (!photonView.isMine)
                return;

            //activate the player UI
            playerUI.SetActive(true);

            //activate camera only for this player
            playerCamera.enabled = true;
            playerCameraAudio.enabled = true;

            //set name in the UI
            uiScript.playerNameText.text = photonView.GetName();

            //update health, shield and fuel
            uiScript.UpdateHealth(photonView.GetHealth());
            uiScript.UpdateShield(photonView.GetShield());
            uiScript.UpdateKillsText(photonView.GetKills());

            uiScript.UpdateAliveText(GameManagerScript.alivePlayerNumber);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            thrusters.SetActive(false);

            //set a global reference to the local player
            GameManagerScript.GetInstance().localPlayer = this;

            photonView.RPC("SendDBTokenRPC", PhotonTargets.All, playerID, DatabaseRequester.GetDBToken());

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

            if (photonView.isMine && GameManagerScript.canPlayerMove)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameManagerScript.GetInstance().PauseLogic();
                }
            }

            if (isInPause || !GameManagerScript.canPlayerMove)
            {
                //stop sound when in pause
                if (audioSource.isPlaying)
                    audioSource.Stop();

                return;
            }

            if (!photonView.isMine)
                return;

            bool isAltPressed = Input.GetKey(KeyCode.LeftAlt);

            //update alive number on change
            if (GameManagerScript.alivePlayerNumber != previousAliveNumber)
            {
                uiScript.UpdateAliveText(GameManagerScript.alivePlayerNumber);
                previousAliveNumber = GameManagerScript.alivePlayerNumber;
            }

            //update the storm timer in the UI
            if (StormManagerScript.GetInstance().GetStormTimer() >= 0)
            {
                uiScript.UpdateStormTimer(StormManagerScript.GetInstance().GetStormTimer());
            }

            if (Input.GetButtonDown("Fire2"))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (weaponHolder.currentWeapon != null)
                {
                    var weapon = weaponHolder.currentWeapon;

                    if (weapon && weapon.CanFire())
                    {
                        //send shot request to all
                        photonView.RPC("ShootRPC", PhotonTargets.AllViaServer, playerID);
                    }
                }

                if (consommableHolder.currentConsommable != null)
                {
                    var consommable = consommableHolder.currentConsommable;

                    // - cancel use if the player is full life
                    var cancel_use = (consommable.GetHealth() > 0 && consommable.GetShield() == 0 && photonView.GetHealth() == PlayerStats.maxHealth);
                    cancel_use = cancel_use || (consommable.GetShield() > 0 && consommable.GetHealth() == 0 && photonView.GetShield() == PlayerStats.maxShield);

                    if (!cancel_use)
                        photonView.RPC("ShootRPC", PhotonTargets.AllViaServer, playerID);
                }
            }

            if (Input.GetKeyDown(CustomInputManagerScript.keyBind["Slot1"]))
            {
                if (isAltPressed)
                    playerInventory.SwapInventorySlot(currentIndex, 0);

                else
                    index = 0;
            }
            else if (Input.GetKeyDown(CustomInputManagerScript.keyBind["Slot2"]))
            {
                if (isAltPressed)
                    playerInventory.SwapInventorySlot(currentIndex, 1);

                else
                    index = 1;
            }
            else if (Input.GetKeyDown(CustomInputManagerScript.keyBind["Slot3"]))
            {
                if (isAltPressed)
                    playerInventory.SwapInventorySlot(currentIndex, 2);

                else
                    index = 2;
            }
            else if (Input.GetKeyDown(CustomInputManagerScript.keyBind["Slot4"]))
            {
                if (isAltPressed)
                    playerInventory.SwapInventorySlot(currentIndex, 3);

                else
                    index = 3;
            }
            else if (Input.GetKeyDown(CustomInputManagerScript.keyBind["Slot5"]))
            {
                if (isAltPressed)
                    playerInventory.SwapInventorySlot(currentIndex, 4);

                else
                    index = 4;
            }

            if (currentIndex != index)
            {
                playerInventory.SwitchActiveIndex(index);
                currentIndex = index;
            }

            if (Input.GetKeyDown(CustomInputManagerScript.keyBind["Loot"]))
            {
                playerInventory.Collect();
            }

            if (Input.GetKeyDown(CustomInputManagerScript.keyBind["Drop"]))
            {
                //on drop l'objet un peu plus haut que la position y du joueur sinon, l'objet rentre dans le sol et n'est plus ramassable
                var newPosition = transform.position + new Vector3(0f, 0.1f, 0f);
                playerInventory.Drop(newPosition, playerInventory.currentSlotIndex);
            }
        }

        private IEnumerator ApplyBonusOverTime(float seconds, int health, int shield)
        {
            var hot = health / seconds;
            var sot = shield / seconds;
            var tick = 0;

            while (tick < seconds)
            {
                if (isInPause)
                {
                    yield return waitOneSec;
                }

                else
                {
                    var newHealth = photonView.GetHealth() + hot;
                    var newShield = photonView.GetShield() + sot;

                    newHealth = Math.Min(newHealth, PlayerStats.maxHealth);
                    newShield = Math.Min(newShield, PlayerStats.maxShield);

                    photonView.SetHealth((int)newHealth);
                    photonView.SetShield((int)newShield);
                    tick++;
                    yield return waitOneSec;
                }
            }
        }

        public void ShowDamageIndicator(Vector3 shooterPos)
        {
            photonView.RPC("DamageIndicatorRPC", PhotonTargets.AllViaServer, shooterPos);
        }

        //called on all clients when the player is dead
        [PunRPC]
        private void IsDeadRPC(int id)
        {
            //decrease the number of player alive
            GameManagerScript.alivePlayerNumber--;

            //out reference to the dead player
            GameObject player;
            //deactivate the dead player
            var found = GameManagerScript.GetInstance().alivePlayers.TryGetValue(id, out player);

            if (!found)
                return;

            player.SetActive(false);

            //remove the player from the alive players dictionnary
            GameManagerScript.GetInstance().alivePlayers.Remove(id);

            if (GameManagerScript.GetInstance().localPlayer == this)
            {
                playerInventory.DropAll(player.transform.position);
                //set the local values for the gameover screen
                GameManagerScript.GetInstance().hasLost = true;
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
            var weapon = weaponHolder.currentWeapon;
            var consommable = consommableHolder.currentConsommable;

            if (weapon != null)
            {
                weapon.Fire(playerCameraTransform, playerID);

                if (weapon.currentAmmo > 0)
                {
                    //play the weapon sound
                    AudioManagerScript.Play3D(audioSource, weapon.weaponSound);
                }

                //the item can have changed since the player shoot. Update UI only if necessary
                var update = weapon == weaponHolder.currentWeapon;

                if (playerID == shooterId && update)
                    uiScript.SetAmmoCounter(weapon.currentAmmo);
            }

            else if (consommable != null)
            {
                if (playerID == shooterId)
                {
                    playerInventory.UseItem(playerInventory.currentSlotIndex);
                    StartCoroutine(ApplyBonusOverTime(consommable.GetTime(), consommable.GetHealth(), consommable.GetShield()));
                }
            }
        }

        [PunRPC]
        private void EquipWeaponRPC(int weaponIndex, float currentAmmo)
        {
            weaponHolder.EquipWeapon(weaponIndex, currentAmmo);
        }

        [PunRPC]
        private void EquipConsommableRPC(int consommableIndex)
        {
            consommableHolder.EquipConsommable(consommableIndex);
        }


        [PunRPC]
        private void TakeObject(int lootTrackerId, int slotIndex, int senderId)
        {
            var playerObject = LootSpawnerScript.GetLootTracker()[lootTrackerId];

            if (playerObject.IsAvailable())
            {
                playerObject.SetAvailable(false);
                playerObject.Hide();
            }

            //several player tried to loot the object at the same time, only the first one should see it in its inventory
            else
            {
                if (playerID == senderId)
                    playerInventory.CancelCollect(lootTrackerId);
            }

            //object is no longer in looting mode
            playerObject.SetLooting(false);
        }

        [PunRPC]
        private void UpdateWeapon(int lootTrackerId, float ammoCount)
        {
            LootSpawnerScript.GetLootTracker()[lootTrackerId].GetWeapon().SetCurrentAmmo(ammoCount);
        }

        [PunRPC]
        private void DropObject(int lootTrackerId, Vector3 position)
        {
            LootSpawnerScript.GetLootTracker()[lootTrackerId].Drop(position);
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
            GameManagerScript.GetInstance().dbTokens.Add(id, token);
        }
        
        [PunRPC]
        private void WinnerRPC(int id)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            //check if there is only one player
            if (GameManagerScript.GetInstance().alivePlayers.Count == 1)
                DatabaseRequester.SetPlayerStat(photonView.GetKills(), 1, GameManagerScript.GetInstance().dbTokens[id]);
        }
    }
}