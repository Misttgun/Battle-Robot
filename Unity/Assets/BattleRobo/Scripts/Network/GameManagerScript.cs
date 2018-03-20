using UnityEngine;
using System.Collections;
using BattleRobo.UI;
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
		[HideInInspector]
		public PlayerScript localPlayer;

		/// <summary>
		/// The list of all the players currently alive in the game.
		/// </summary>
		public List<GameObject> alivePlayers;
		//TODO Utiliser des dictionnary à la place de la liste. Cela sera plus simple pour les RPCs au moment de tuer les joueurs.

		/// <summary>
		/// Reference to the game over UI.
		/// </summary>
		[SerializeField]
		private GameObject gameOverUI;

		/// <summary>
		/// Reference to the game over UI script.
		/// </summary>
		[SerializeField]
		private GameOverUIScript gameOverUiScript;

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
			alivePlayers = new List<GameObject>(8);
		}

		private void Start()
		{
			alivePlayerNumber = PhotonNetwork.room.PlayerCount;
			gameOverUI.SetActive(false);
			gameCamera.SetActive(false);

			PhotonNetwork.Instantiate("Robo", Vector3.zero, Quaternion.identity, 0);
		}

		private void Update()
		{
			if (IsGameWon())
			{
				ShowGameOverScreen("You won !! Let's go baby !!");
			}
			else if (IsGameLost())
			{
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
			bool isWon = false;

			if (alivePlayerNumber == 1 && alivePlayers.Contains(localPlayer.gameObject))
			{
				isWon = true;
			}

			//return the result
			return isWon;
		}

		/// <summary>
		/// Returns whether the game is lost by the local player.
		/// </summary>
		public bool IsGameLost()
		{
			//init variables
			bool isLost = false;

			if (alivePlayerNumber >= 1 && !alivePlayers.Contains(localPlayer.gameObject))
			{
				isLost = true;
			}

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