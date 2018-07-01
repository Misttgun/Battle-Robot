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
            photonView.SetShield(maxHealth);
            photonView.SetKills(0);
        }

        private void Start()
        {
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

            if (Input.GetButtonDown("Pause"))
            {
                int counter;
                bool found = GameManagerScript.GetInstance().pauseCounter.TryGetValue(playerID, out counter);

                if (!isInPause)
                {
                    // - set current player in pause
                    GameManagerScript.GetInstance().SetPlayerInPause(playerID);

                    // - increment pause counter
                    if (found)
                        GameManagerScript.GetInstance().pauseCounter[playerID]++;

                    // - init pause counter if necessary
                    else
                        GameManagerScript.GetInstance().pauseCounter[playerID] = 0;

                    // - max pause duration is 10 sec
                    //TODO remplacer le Invoke
                    //Invoke("PauseTimeout", 10);
                }

                // - dispatch pause if the player haven't used it more than 3 times or if the game is already in pause
                if (isInPause && GameManagerScript.GetInstance().GetPlayerInPause() == playerID || !isInPause && counter < 2)
                {
                    photonView.RPC(!isInPause ? "SetPause" : "CancelPause", PhotonTargets.AllViaServer);
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

            //update alive number on change
            if (GameManagerScript.alivePlayerNumber != previousAliveNumber)
            {
                uiScript.UpdateAliveText(GameManagerScript.alivePlayerNumber);
                previousAliveNumber = GameManagerScript.alivePlayerNumber;
            }

            //update the storm timer in the UI
            if (StormManagerScript.GetInstance().GetStormTimer() >= 0)
            {
                uiScript.UpdateStormTimer(StormManagerScript.GetInstance().GetStormTimer() + 1);
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
            }

            if (Input.GetButtonDown("Inventory1"))
            {
                index = 0;
            }
            else if (Input.GetButtonDown("Inventory2"))
            {
                index = 1;
            }
            else if (Input.GetButtonDown("Inventory3"))
            {
                index = 2;
            }
            else if (Input.GetButtonDown("Inventory4"))
            {
                index = 3;
            }
            else if (Input.GetButtonDown("Inventory5"))
            {
                index = 4;
            }

            if (currentIndex != index)
            {
                playerInventory.SwitchActiveIndex(index);
                currentIndex = index;
            }

            if (Input.GetButtonDown("Loot"))
            {
                playerInventory.Collect();
            }

            if (Input.GetButtonDown("Drop"))
            {
                //on drop l'objet un peu plus haut que la position y du joueur sinon, l'objet rentre dans le sol et n'est plus ramassable
                var newPositipn = transform.position + new Vector3(0f, 0.1f, 0f);
                playerInventory.Drop(newPositipn);
            }
        }

        //TODO déplacer la méthode dans une classe statique
        private void SetPlayerStats(int kills, int win, string token)
        {
            string url = "http://51.38.235.234:8080/update_player?token=" + token + "&kill=" + kills + "&win=" + win;

            // don't wait for response
            WWW www = new WWW(url);
        }

        private void PauseTimeout()
        {
            // - the master client shall do the Timeout RPC to avoid that a corrupted client keep game in pause forever
            bool isMasterClient = PhotonNetwork.player.IsMasterClient;

            // - if still in pause, master client will send an RPC to exit pause
            if (isMasterClient && isInPause)
                photonView.RPC("CancelPause", PhotonTargets.AllViaServer);
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
    }
}