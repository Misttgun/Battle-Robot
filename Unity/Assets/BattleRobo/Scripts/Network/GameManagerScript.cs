using UnityEngine;
using BattleRobo.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
		public PlayerScript localPlayer;

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
		/// Reference to the game camera to show the game over UI.
		/// </summary>
		[SerializeField]
		private GameObject gameCamera;

		// Temporary list of spawn points for the player
		public Transform[] spawnPoints;

		// Number of player currently alive in the game
		public int alivePlayerNumber;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;

			//we reserve 8 spots because that's the maximum number of players for now
			alivePlayers = new Dictionary<int, GameObject>(8);
		}

		private void Start()
		{
			alivePlayerNumber = PhotonNetwork.room.PlayerCount;
			gameCamera.SetActive(false);

			PhotonNetwork.Instantiate("Robo", new Vector3(0, 10, 7), Quaternion.identity, 0);
		}

		private void Update()
		{
			if (IsGameWon())
			{
				//desactivate the local player
				localPlayer.gameObject.SetActive(false);
				
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				ShowGameOverScreen("You won !! Let's go baby !!");
			}
			else if (IsGameLost())
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				ShowGameOverScreen("You died... Feels bad man.");
			}
		}

		/// <summary>
		/// Returns a reference to this script instance.
		/// </summary>
		public static GameManagerScript GetInstance()
		{
			return Instance;
		}

		/// <summary>
		/// Returns whether the game is won by the local player.
		/// </summary>
		public bool IsGameWon()
		{
			//init variables
			bool isWon = alivePlayerNumber == 1 && alivePlayers.ContainsKey(localPlayer.playerID);

			//return the result
			return isWon;
		}

		/// <summary>
		/// Returns whether the game is lost by the local player.
		/// </summary>
		public bool IsGameLost()
		{
			//init variables
			bool isLost = alivePlayerNumber >= 1 && !alivePlayers.ContainsKey(localPlayer.playerID);

			//return the result
			return isLost;
		}

		public void ShowGameOverScreen(string goText)
		{
			gameCamera.SetActive(true);
			gameOverUI.SetActive(true);
			gameOverUiScript.UpdateGameOverText(goText);
		}
	}
}