using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRobo.Networking
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

		private bool isMoreThanTwo;

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
				else
				{
					// Change the loading message
					loadingMessageLabel.text = "Waiting for more players...";
					isMoreThanTwo = false;
				}
			}
		}


		public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
		{
			Debug.Log("BattleRobo/Launcher: OnPhotonRandomJoinFailed() was called by PUN");

			// We failed to join a random room, we create a new room.
			PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom }, null);
		}

		public override void OnJoinedRoom()
		{
			Debug.Log("BattleRobo/Launcher: OnJoinedRoom() called by PUN");

			// Show the loading screen
			startMenuCanvas.SetActive(false);
			loadingScreenCanvas.SetActive(true);

			if (PhotonNetwork.isMasterClient)
			{
				StartCoroutine(LoadGame());
			}
		}


		/// <summary>
		/// Start a new match.
		/// </summary>
		public void Play()
		{
			if (PhotonNetwork.inRoom)
			{
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


		private IEnumerator LoadGame()
		{
			// TODO Delete while true and find a better way
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
				yield return new WaitForSeconds(1f);
			}

			// Close the room so that a new player can not join the game
			PhotonNetwork.room.IsOpen = false;
			PhotonNetwork.room.IsVisible = false;

			Debug.Log("Load level");
			// We load the game
			PhotonNetwork.LoadLevel(2);
		}

		private void SetPlayerNumbersLabel(int number)
		{
			playerNumbersLabel.text = "Numbers of players : " + number;
		}
	}
}