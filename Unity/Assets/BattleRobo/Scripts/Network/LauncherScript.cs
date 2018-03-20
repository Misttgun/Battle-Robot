using UnityEngine;

namespace BattleRobo.Networking
{
    public class LauncherScript : Photon.PunBehaviour
    {
        // The Ui Panel to let the user enter name, connect and play
        public GameObject controlPanel;

        // The UI Label to inform the user that the connection is in progress
        public GameObject progressLabel;

		/// <summary>
		/// Game version. Only players with the same version will find each other.
		/// </summary>
        private const string GameVersion = "0.2";


        private void Awake()
        {
            // We join the default lobby.
            PhotonNetwork.autoJoinLobby = true;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("BattleRobo/Launcher: OnJoinedLobby() was called by PUN");

            // If we are in the lobby, we launch the main menu
            PhotonNetwork.LoadLevel(1);
        }

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt to join a random room.
        /// - Esle, connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

			// We try to connect to server
			PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.BestRegion;
			PhotonNetwork.ConnectToBestCloudServer(GameVersion);
        }
    }
}