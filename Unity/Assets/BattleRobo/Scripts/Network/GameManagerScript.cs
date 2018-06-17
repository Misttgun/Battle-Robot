using UnityEngine;
using System.Collections.Generic;
using BattleRobo.Scripts.Network;
using UnityEngine.SceneManagement;

namespace BattleRobo
{
    /// <summary>
    /// Manages game workflow and provides high-level access to networked logic during a game.
    /// It manages functions such as win and loose situation.
    /// </summary>
	public class GameManagerScript : Photon.PunBehaviour, IPunObservable
    {
        // reference to this script instance
        private static GameManagerScript Instance;

        /// <summary>
        /// The local player instance spawned for this client.
        /// </summary>
        //[HideInInspector]
        //public PlayerScript localPlayer;
        public RoboControllerScript localPlayer;

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
        /// Reference to the network command gameobject.
        /// </summary>
        public GameObject networkCommandObject;
        
        // Number of player currently alive in the game
        public static int alivePlayerNumber;

        public int pRank;

        public bool hasLost;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            //we reserve 4 spots because that's the maximum number of players for now
            alivePlayers = new Dictionary<int, GameObject>(4);
            pauseCounter = new Dictionary<int, int>(4);
            dbTokens = new Dictionary<int, string>(4);
        }

        private void Start()
        {
            alivePlayerNumber = PhotonNetwork.room.PlayerCount;
            gameCamera.SetActive(false);
            
            pRank = alivePlayerNumber;
            hasLost = false;
        }

        private void Update()
        {
            if (!localPlayer)
                return;

            if (!hasLost && alivePlayerNumber == 1)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                ShowGameOverScreen("You won !! Let's go baby !!", 1, localPlayer.playerStats.Kills);
                
                //desactivate the local player
                photonView.RPC("DisablePlayerRPC", PhotonTargets.All, localPlayer.playerID);
            }
            else if (hasLost)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                ShowGameOverScreen("You died... Feels bad man.", pRank, localPlayer.playerStats.Kills);
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
            pauseTimer = timer;

            if (timer < 0f)
                timer = 0f;
        }

        public float GetPauseTimer()
        {
            return pauseTimer;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.isWriting)
			{
				stream.SendNext(alivePlayerNumber);
			}
			else
			{
				alivePlayerNumber = (int)stream.ReceiveNext();
			}
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
	}
}