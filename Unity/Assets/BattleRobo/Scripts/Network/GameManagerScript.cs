using UnityEngine;
using System.Collections.Generic;

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
        public RoboControllerScript localPlayer;

        /// <summary>
        /// The dictionnary of all the players currently alive in the game.
        /// </summary>
        public Dictionary<int, GameObject> alivePlayers;

        /// <summary>
        /// Reference to the game over UI.
        /// </summary>
        public GameObject gameOverUI;

        /// <summary>
        /// Reference to the game over UI script.
        /// </summary>
        public GameOverUIScript gameOverUiScript;

        /// <summary>
        /// Reference to the game over UI script.
        /// </summary>
        private bool isGamePause;

        /// <summary>
        /// Reference to the game camera to show the game over UI.
        /// </summary>
        [SerializeField]
        private GameObject gameCamera;
        
        // Number of player currently alive in the game
        public static int alivePlayerNumber;

        public int pRank, pKills;

        public bool hasLost;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            //we reserve 4 spots because that's the maximum number of players for now
            alivePlayers = new Dictionary<int, GameObject>(4);
        }

        private void Start()
        {
            alivePlayerNumber = PhotonNetwork.room.PlayerCount;
            gameCamera.SetActive(false);
            
            pRank = alivePlayerNumber;
            pKills = 0;

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
                ShowGameOverScreen("You won !! Let's go baby !!", 1, pKills);

                //desactivate the local player
                photonView.RPC("DisablePlayerRPC", PhotonTargets.All, localPlayer.playerID);
            }
            else if (hasLost)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                ShowGameOverScreen("You died... Feels bad man.", pRank, pKills);
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