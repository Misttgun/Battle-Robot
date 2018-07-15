using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace BattleRobo
{
    public class LobbyScript : Photon.PunBehaviour
    {
        // The maximum number of players per room.
        private const byte maxPlayersPerRoom = 4;

        [SerializeField]
        private MainMenuScript mainMenuScript;

        [SerializeField]
        private int countDowntimer;

        private int playerNumber;

        private readonly Hashtable test = new Hashtable();

        private readonly WaitForSeconds timer = new WaitForSeconds(1f);


        private void Start()
        {
            PhotonNetwork.autoCleanUpPlayerObjects = false;
            PhotonNetwork.automaticallySyncScene = true;

            test.Add("timer", countDowntimer);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            // We failed to join a random room, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayersPerRoom}, null);
        }

        public override void OnJoinedRoom()
        {
            photonView.RPC("OnRoomJoinedRPC", PhotonTargets.All);

            // Show the loading screen
            mainMenuScript.ShowLoadingScreen();

            if (PhotonNetwork.isMasterClient)
            {
                int seed = 42;
                Hashtable properties = new Hashtable {{"seed", seed}, {"timer", countDowntimer}};
                PhotonNetwork.room.SetCustomProperties(properties);
            }
        }

        /// <summary>
        /// Called when a remote player left the room.
        /// </summary>
        public override void OnPhotonPlayerDisconnected(PhotonPlayer player)
        {
            if (PhotonNetwork.player.Equals(player))
                return;
            //tell all clients that the player is dead
            photonView.RPC("PlayerLeftRPC", PhotonTargets.All);
        }

        /// <summary>
        /// Called after disconnecting from the Photon server.
        /// </summary>
        public override void OnDisconnectedFromPhoton()
        {
            DatabaseRequester.Logout();
            DatabaseRequester.SetDBToken(null);
            DatabaseRequester.SetPseudo(null);

            //switch from the online to the offline scene after connection is closed
            if (SceneManager.GetActiveScene().buildIndex != 0)
                SceneManager.LoadScene(0);
        }

        public override void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
        {
            mainMenuScript.redyCountDownLabel.text = Convert.ToInt32(propertiesThatChanged["timer"]).ToString();
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
            DatabaseRequester.Logout();
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
            mainMenuScript.ShowMainMenu();
        }


        private IEnumerator LoadGame()
        {
            // We wait 10 seconds
            int countDown = countDowntimer;
            while (countDown > 0)
            {
                // We stop the countdown when the room is full
                if (PhotonNetwork.room.PlayerCount == maxPlayersPerRoom)
                {
                    break;
                }

                --countDown;
                test["timer"] = countDown;
                PhotonNetwork.room.SetCustomProperties(test);
                yield return timer;
            }

            // Close the room so that a new player can not join the game
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.room.IsVisible = false;

            // We load the game
            PhotonNetwork.LoadLevel(2);
        }

        [PunRPC]
        private void OnRoomJoinedRPC()
        {
            playerNumber = PhotonNetwork.room.PlayerCount;
            // Set the number of player on the loading screen
            mainMenuScript.SetPlayerNumbersLabel(playerNumber);

            UpdateLoadingUI();

            if (playerNumber == 2 && PhotonNetwork.isMasterClient)
            {
                StartCoroutine(LoadGame());
            }
        }

        [PunRPC]
        private void PlayerLeftRPC()
        {
            playerNumber = PhotonNetwork.room.PlayerCount;
            // Set the number of player on the loading screen
            mainMenuScript.SetPlayerNumbersLabel(playerNumber);

            UpdateLoadingUI();

            if (playerNumber < 2 && PhotonNetwork.isMasterClient)
            {
                StopAllCoroutines();
            }
        }

        private void UpdateLoadingUI()
        {
            // We get out of this loop when we have 2 or more players
            if (playerNumber >= 2)
            {
                // Change the loading message
                mainMenuScript.loadingMessageLabel.SetActive(false);
                mainMenuScript.readyMessageLabel.SetActive(true);
                mainMenuScript.cancelButton.interactable = false;
            }
            else
            {
                // Change the loading message
                mainMenuScript.loadingMessageLabel.SetActive(true);
                mainMenuScript.readyMessageLabel.SetActive(false);
                mainMenuScript.cancelButton.interactable = true;
            }
        }
    }
}