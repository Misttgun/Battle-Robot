using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngineInternal;

namespace BattleRobo
{
    /// <summary>
    /// Manages game workflow and provides high-level access to networked logic during a game.
    /// It manages functions such as win and loose situation.
    /// </summary>
    public class GameManagerScript : Photon.PunBehaviour
    {
        // reference to this script instance
        private static GameManagerScript Instance;

        /// <summary>
        /// The local player instance spawned for this client.
        /// </summary>
        //[HideInInspector]
        //public PlayerScript localPlayer;
        public RoboLogicScript localPlayer;

        /// <summary>
        /// The dictionnary of all the players currently alive in the game.
        /// </summary>
        public Dictionary<int, GameObject> alivePlayers;

        // - masterClient only : <photonID, dbToken>
        public Dictionary<int, string> dbTokens;

        /// <summary>
        /// The dictionnary of pause used by players.
        /// </summary>
        public Dictionary<int, int> pauseCounter;

        /// <summary>
        /// Reference to the game over UI.
        /// </summary>
        public GameObject gameOverUI;

        /// <summary>
        /// Reference to the game over UI script.
        /// </summary>
        public GameOverUIScript gameOverUiScript;

        /// <summary>
        /// Current pause state of the game
        /// </summary>
        private bool isGamePause;

        /// <summary>
        /// Reference the player who put the game in pause
        /// </summary>
        private int playerInPauseId;

        /// <summary>
        /// Pause timer
        /// </summary>
        private float pauseTimer;

        /// <summary>
        /// Reference to the game camera to show the game over UI.
        /// </summary>
        [SerializeField]
        private GameObject gameCamera;

        /// <summary>
        /// Reference to the waiting screen game object.
        /// </summary>
        [SerializeField]
        private GameObject waitingScreen;

        /// <summary>
        /// Reference to the spawnGenerator Script.
        /// </summary>
        public SpawnGeneratorScript spawnGenerator;


        public static bool canPlayerMove;

        // Number of player currently alive in the game
        public static int alivePlayerNumber;

        private int readyCount;
        public static bool ready;

        public bool hasLost;

        private bool deactivate;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            //we reserve 4 spots because that's the maximum number of players for now
            alivePlayers = new Dictionary<int, GameObject>(4);
            pauseCounter = new Dictionary<int, int>(4);
            dbTokens = new Dictionary<int, string>(4);

            //set the ready count
            readyCount = 0;
        }

        private void Start()
        {
            PhotonNetwork.Instantiate("Robo_Pred", spawnGenerator.spawnPositions[PhotonNetwork.player.ID - 1], Quaternion.identity, 0);

            //tell the master client that we are ready
            photonView.RPC("PlayerReadyRPC", PhotonTargets.MasterClient);

            alivePlayerNumber = PhotonNetwork.room.PlayerCount;
            gameCamera.SetActive(false);

            hasLost = false;
        }

        private void Update()
        {
            if (!localPlayer)
                return;

            if (isGamePause)
                SetPauseTimer(pauseTimer - Time.deltaTime);

            //TODO Utiliser des delegate ou de events pour les codes à executer une seule fois

            if (!hasLost && alivePlayerNumber == 1)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                ShowGameOverScreen("You won !! Let's go baby !!", 1, localPlayer.photonView.GetKills());

                localPlayer.photonView.RPC("WinnerRPC", PhotonTargets.MasterClient, localPlayer.playerID);
                
                //desactivate the local player
                photonView.RPC("DisablePlayerRPC", PhotonTargets.All, localPlayer.playerID);
            
                deactivate = true;
            }
            else if (hasLost)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                ShowGameOverScreen("You died... Feels bad man.", localPlayer.photonView.GetRank(), localPlayer.photonView.GetKills());

                deactivate = true;
            }

            if (deactivate)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Returns a reference to this script instance.
        /// </summary>
        public static GameManagerScript GetInstance()
        {
            return Instance;
        }

        private void ShowGameOverScreen(string goText, int rank, int kills)
        {
            gameCamera.SetActive(true);
            gameOverUI.SetActive(true);
            gameOverUiScript.UpdateGameOverText(goText, rank, kills);
        }

        public bool IsGamePause()
        {
            return isGamePause;
        }

        public void SetPause(bool pause)
        {
            isGamePause = pause;
            pauseTimer = 10f;
        }

        public void SetPlayerInPause(int idPlayer)
        {
            playerInPauseId = idPlayer;
        }

        public int GetPlayerInPause()
        {
            return playerInPauseId;
        }

        public void SetPauseTimer(float timer)
        {
            if (timer < 0f)
                timer = 0f;

            pauseTimer = timer;
        }

        public float GetPauseTimer()
        {
            return pauseTimer;
        }

        /// <summary>
        /// Called when a remote player left the room.
        /// </summary>
        public override void OnPhotonPlayerDisconnected(PhotonPlayer player)
        {
            //tell all clients that the player is dead
            photonView.RPC("HasLeftRPC", PhotonTargets.All, player.ID);
        }

        /// <summary>
        /// Called after disconnecting from the Photon server.
        /// </summary>
        public override void OnDisconnectedFromPhoton()
        {
            //switch from the online to the offline scene after connection is closed
            if (SceneManager.GetActiveScene().buildIndex != 0)
                SceneManager.LoadScene(0);

            var playerToken = PlayerInfoScript.GetInstance().GetDBToken();
            DatabaseRequester.GetInstance().AsyncQuery("logout?token="+playerToken);
        }

        //called on all clients when the player left the room
        [PunRPC]
        private void HasLeftRPC(int id)
        {
            //remove the player from the alive players dictionnary and decrease the number of player alive
            //out reference to the dead player
            GameObject player;

            //deactivate the dead player
            var found = alivePlayers.TryGetValue(id, out player);

            if (found)
            {
                player.SetActive(false);
            }

            alivePlayers.Remove(id);
            alivePlayerNumber--;
        }

        [PunRPC]
        private void DisablePlayerRPC(int id)
        {
            //out reference to the dead player
            GameObject player;

            //deactivate the dead player
            var found = alivePlayers.TryGetValue(id, out player);

            if (found)
            {
                player.SetActive(false);
            }
        }

        [PunRPC]
        private void PlayerReadyRPC()
        {
            readyCount++;

            if (readyCount == alivePlayerNumber)
                StartCoroutine(WaitingScreenTimer());
        }

        [PunRPC]
        private void MatchBeginRPC()
        {
            waitingScreen.SetActive(false);
            canPlayerMove = true;
        }

        private IEnumerator WaitingScreenTimer()
        {
            yield return new WaitForSeconds(5f);

            ready = true;
            photonView.RPC("MatchBeginRPC", PhotonTargets.AllViaServer);
        }
    }
}