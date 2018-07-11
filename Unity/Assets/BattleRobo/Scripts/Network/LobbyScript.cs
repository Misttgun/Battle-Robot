using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace BattleRobo
{
    public class LobbyScript : Photon.PunBehaviour
    {
        // The maximum number of players per room.
        private const byte maxPlayersPerRoom = 4;

        [SerializeField]
        private GameObject startMenuCanvas;

        [SerializeField]
        private GameObject loadingScreenCanvas;

        [SerializeField]
        private Text playerNumbersLabel;

        [SerializeField]
        private Text loadingMessageLabel;

        [SerializeField]
        private Button cancelButton;

        [SerializeField]
        private GameObject lobbyPanel;

        [SerializeField]
        private GameObject leaderboardPanel;

        [SerializeField]
        private List<GameObject> leaderboardRows;

        private bool isMoreThanTwo;

        private readonly WaitForSeconds timer = new WaitForSeconds(1f);

        private void Start()
        {
            startMenuCanvas.SetActive(true);
            loadingScreenCanvas.SetActive(false);

            PhotonNetwork.autoCleanUpPlayerObjects = false;
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
                    cancelButton.interactable = false;
                    isMoreThanTwo = true;
                }
                else
                {
                    // Change the loading message
                    loadingMessageLabel.text = "Waiting for more players...";
                    cancelButton.interactable = true;
                    isMoreThanTwo = false;
                }
            }
        }


        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            // We failed to join a random room, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayersPerRoom}, null);
        }

        public override void OnJoinedRoom()
        {
            // Show the loading screen
            startMenuCanvas.SetActive(false);
            loadingScreenCanvas.SetActive(true);

            if (PhotonNetwork.isMasterClient)
            {
                int seed = 42;
                Hashtable properties = new Hashtable {{"seed", seed}};
                PhotonNetwork.room.SetCustomProperties(properties);
                StartCoroutine(LoadGame());
            }
        }


        /// <summary>
        /// Start a new match.
        /// </summary>
        public void Play()
        {
            //play the waiting music
            AudioManagerScript.PlayMusic(0);
            if (PhotonNetwork.inRoom)
            {
                // Close the room so that a new player can not join the game
                PhotonNetwork.room.IsOpen = false;
                PhotonNetwork.room.IsVisible = false;

                PhotonNetwork.LeaveRoom();
            }

            // We try to join a random room
            PhotonNetwork.JoinRandomRoom();
        }

        /// <summary>
        /// Quit the game
        /// </summary>
        public void Quit()
        {
            // We quit the application
            Application.Quit();
        }

        /// <summary>
        /// Cancel matchmaking and return to lobby
        /// </summary>
        public void Cancel()
        {
            //stop the waiting music
            AudioManagerScript.GetInstance().musicSource.Stop();

            StopAllCoroutines();

            // Close the room so that a new player can not join the game
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.room.IsVisible = false;

            // We leave the room
            PhotonNetwork.LeaveRoom();

            // We show the main menu
            startMenuCanvas.SetActive(true);
            loadingScreenCanvas.SetActive(false);
        }


        private IEnumerator LoadGame()
        {
            // TODO Delete while true and find a better way -> Use an RPC to the master client
            // Waiting for one more player
            while (true)
            {
                if (isMoreThanTwo)
                {
                    break;
                }

                yield return null;
            }

            // We wait 10 seconds
            int countDown = 4;
            while (countDown > 0)
            {
                // We stop the countdown when the room is full
                if (PhotonNetwork.room.PlayerCount == maxPlayersPerRoom)
                {
                    break;
                }

                countDown--;
                yield return timer;
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

        public void GoToLeaderboardPanel()
        {
            lobbyPanel.SetActive(false);
            leaderboardPanel.SetActive(true);
            FillLeaderboard();
        }

        public void GoToLobbyPanel()
        {
            lobbyPanel.SetActive(true);
            leaderboardPanel.SetActive(false);
            
        }

        private void FillLeaderboard()
        {
            int status;
            string response;

            DatabaseRequester.GetInstance().SyncQuery("/leaderboard", out status, out response);

            // - leaderboard is loaded successfully
            if (status == 200)
            {
                var csv_text = response;
                var rows = csv_text.Split('\n');

                for (int i = 0; i < rows.Length; i++)
                {
                    var row = rows[i].Split(',');

                    if (row.Length == 4 && i < leaderboardRows.Count)
                    {
                        GameObject leaderboardRow = leaderboardRows[i];

                        Text playerText = (Text)leaderboardRow.transform.GetChild(1).GetComponent("Text");
                        Text killText = (Text)leaderboardRow.transform.GetChild(2).GetComponent("Text");
                        Text winText = (Text)leaderboardRow.transform.GetChild(3).GetComponent("Text");
                        Text scoreText = (Text)leaderboardRow.transform.GetChild(4).GetComponent("Text");

                        playerText.text = row[0];
                        winText.text = row[1];
                        killText.text = row[2];
                        scoreText.text = row[3];
                    }
                }
            }

            else
            {
                // TODO ERROR MESSAGE
            }
        }
    }
}