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
        private MainMenuScript mainMenuScript;

        private bool isMoreThanTwo;

        private readonly WaitForSeconds timer = new WaitForSeconds(1f);

        private void Start()
        {
            PhotonNetwork.autoCleanUpPlayerObjects = false;
            PhotonNetwork.automaticallySyncScene = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

//        private void Update()
//        {
//            if (PhotonNetwork.inRoom)
//            {
//                // Set the number of player on the loading screen
//                mainMenuScript.SetPlayerNumbersLabel(PhotonNetwork.room.PlayerCount);
//
//                // We get out of this loop when we have 2 or more players
//                if (PhotonNetwork.room.PlayerCount >= 2)
//                {
//                    // Change the loading message
//                    mainMenuScript.loadingMessageLabel.text = "The game is starting...";
//                    mainMenuScript.cancelButton.interactable = false;
//                    isMoreThanTwo = true;
//                }
//                else
//                {
//                    // Change the loading message
//                    mainMenuScript.loadingMessageLabel.text = "Waiting for more players...";
//                    mainMenuScript.cancelButton.interactable = true;
//                    isMoreThanTwo = false;
//                }
//            }
//        }


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
                Hashtable properties = new Hashtable {{"seed", seed}};
                PhotonNetwork.room.SetCustomProperties(properties);
                StartCoroutine(LoadGame());
            }
        }

        /// <summary>
        /// Called after disconnecting from the Photon server.
        /// </summary>
        public override void OnDisconnectedFromPhoton()
        {
            var playerToken = PlayerInfoScript.GetInstance().GetDBToken();
            DatabaseRequester.GetInstance().AsyncQuery("/logout?token=" + playerToken);
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
            var playerToken = PlayerInfoScript.GetInstance().GetDBToken();
            DatabaseRequester.GetInstance().AsyncQuery("/logout?token=" + playerToken);
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

        [PunRPC]
        private void OnRoomJoinedRPC()
        {
            // Set the number of player on the loading screen
            mainMenuScript.SetPlayerNumbersLabel(PhotonNetwork.room.PlayerCount);

            // We get out of this loop when we have 2 or more players
            if (PhotonNetwork.room.PlayerCount >= 2)
            {
                // Change the loading message
                mainMenuScript.loadingMessageLabel.text = "The game is starting...";
                mainMenuScript.cancelButton.interactable = false;
                isMoreThanTwo = true;
            }
            else
            {
                // Change the loading message
                mainMenuScript.loadingMessageLabel.text = "Waiting for more players...";
                mainMenuScript.cancelButton.interactable = true;
                isMoreThanTwo = false;
            }
        }
    }
}