using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace BattleRobo.Networking
{
    public class StartMenuScript : Photon.PunBehaviour
    {
        #region Public Variables

        #endregion

        #region Private Variables

        // The maximum number of players per room.
        private const byte maxPlayersPerRoom = 8;

        [SerializeField]
        private GameObject startMenuCanvas;

        [SerializeField]
        private GameObject loadingScreenCanvas;

        [SerializeField]
        private Text playerNumbersLabel;

        [SerializeField]
        private Text loadingMessageLabel;
        
        private bool isMoreThanTwo;

        #endregion

        #region Monobehaviour Callbacks

        private void Start()
        {
            startMenuCanvas.SetActive(true);
            loadingScreenCanvas.SetActive(false);

            PhotonNetwork.automaticallySyncScene = true;
        }

        private void Update()
        {
            if (PhotonNetwork.inRoom)
            {
                // Set the number of player on the loading screen
                SetPlayerNumbersLabel(PhotonNetwork.room.PlayerCount);

                // We get out of this loop when we have 2 or more players
                if (PhotonNetwork.room.PlayerCount >= 2)
                {
                    // Change the loading message
                    loadingMessageLabel.text = "The game is starting...";

                    isMoreThanTwo = true;
                }
            }
        }

        #endregion

        #region Photon.PunBehaviour Callbacks

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("BattleRobo/Launcher: OnPhotonRandomJoinFailed() was called by PUN");

            // We failed to join a random room, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayersPerRoom}, null);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("BattleRobo/Launcher: OnJoinedRoom() called by PUN");

            // Show the loading screen
            startMenuCanvas.SetActive(false);
            loadingScreenCanvas.SetActive(true);

            if (PhotonNetwork.isMasterClient && isMoreThanTwo)
            {
                StartCoroutine(LoadGame());
            }
        }

        public override void OnLeftRoom()
        {
            Application.Quit();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start a new match.
        /// </summary>
        public void Play()
        {
            // We try to join a random room
            PhotonNetwork.JoinRandomRoom();
        }

        /// <summary>
        /// Quit the game
        /// </summary>
        public void Quit()
        {
            // We leave the room
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Private Methods

        private IEnumerator LoadGame()
        {
            // We wait 10 seconds
            int countDown = 10;
            while (countDown > 0)
            {
                // We stop the countdown when the room is full
                if (PhotonNetwork.room.PlayerCount == maxPlayersPerRoom)
                {
                    break;
                }

                countDown--;
                yield return new WaitForSeconds(1f);
            }

            // Close the room so that a new player can not join the game
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.room.IsVisible = false;

            // We load the game
            PhotonNetwork.LoadLevel(2);
        }

        private void SetPlayerNumbersLabel(int number)
        {
            playerNumbersLabel.text = "Numbers of players : " + number;
        }

        #endregion
    }
}