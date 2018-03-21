using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace BattleRobo
{
	/// <summary>
	/// Custom implementation of the most Photon callback handlers for network workflows.
	/// This script handles failed connection and disconnection.
	/// </summary>
	public class NetworkManagerScript : PunBehaviour
	{
		//reference to this script instance
		private static NetworkManagerScript Instance;

		/// <summary>
		/// Scene index that gets loaded when player disconnect from a game.
		/// </summary>
		public int offlineSceneIndex = 0;

		/// <summary>
		/// Scene index that gets loaded after a connection has been established.
		/// </summary>
		public int onlineSceneIndex = 1;

		/// <summary>
		/// Maximum amount of players per room.
		/// </summary>
		public int maxPlayers = 8;

		/// <summary>
		/// References to the available player prefabs located in a Resources folder.
		/// </summary>
		public GameObject[] playerPrefabs;

		/// <summary>
		/// Event fired when a connection to the matchmaker service failed.
		/// </summary>
		public static event Action connectionFailedEvent;

		//initialize network view
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else if (Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			//adding a view to this gameobject with a unique viewID
			//this is to avoid having the same ID in a scene
			PhotonView view = gameObject.AddComponent<PhotonView>();
			view.viewID = 999;
		}


		/// <summary>
		/// Returns a reference to this script instance.
		/// </summary>
		public static NetworkManagerScript GetInstance()
		{
			return Instance;
		}


		/// <summary>
		/// Called if a connect call to the Photon server failed before the connection was established.
		/// </summary>
		public override void OnFailedToConnectToPhoton(DisconnectCause cause)
		{
			if (connectionFailedEvent != null)
				connectionFailedEvent();
		}


		/// <summary>
		/// Called when something causes the connection to fail (after it was established).
		/// </summary>
		public override void OnConnectionFail(DisconnectCause cause)
		{
			if (connectionFailedEvent != null)
				connectionFailedEvent();
		}


		/// <summary>
		/// Called when a remote player left the room.
		/// </summary>
		public override void OnPhotonPlayerDisconnected(PhotonPlayer player)
		{
			//tell all clients that the player is dead
			photonView.RPC("HasLeftRPC", PhotonTargets.All, player.ID);

			//get the player back in the lobby
			SceneManager.LoadScene(onlineSceneIndex);
		}


		/// <summary>
		/// Called after disconnecting from the Photon server.
		/// </summary>
		public override void OnDisconnectedFromPhoton()
		{
			//switch from the online to the offline scene after connection is closed
			if (SceneManager.GetActiveScene().buildIndex != offlineSceneIndex)
				SceneManager.LoadScene(offlineSceneIndex);
		}

		//called on all clients when the player left the room
		[PunRPC]
		private void HasLeftRPC(int id)
		{
			//remove the player from the alive players dictionnary and decrease the number of player alive
			GameManagerScript.GetInstance().alivePlayers.Remove(id);
			GameManagerScript.GetInstance().alivePlayerNumber--;
		}
	}
}