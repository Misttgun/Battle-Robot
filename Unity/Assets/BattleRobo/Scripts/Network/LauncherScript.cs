using UnityEngine;

namespace BattleRobo.Networking
{
    public class LauncherScript : Photon.PunBehaviour
    {
        // The Ui Panel to let the user enter name, connect and play
        public GameObject controlPanel;

        // The UI Label to inform the user that the connection is in progress
        public GameObject progressLabel;

        // The PUN loglevel. 
        public PhotonLogLevel loglevel = PhotonLogLevel.Informational;

        // This client's version number. Users are separated from each other by gameversion.
        private const string GameVersion = "0.1";


        private void Awake()
        {
            // We join the default lobby.
            PhotonNetwork.autoJoinLobby = true;

            // Force LogLevel
            PhotonNetwork.logLevel = loglevel;
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
            PhotonNetwork.ConnectUsingSettings(GameVersion);
        }
    }
}