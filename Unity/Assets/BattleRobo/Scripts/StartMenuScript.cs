using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleRobo.Networking
{
    public class StartMenuScript : Photon.PunBehaviour
    {
        #region Public Variables

        // The maximum number of players per room.
        public byte maxPlayersPerRoom = 8;

        // The Ui Panel to let the user enter name, connect and play
        public GameObject controlPanel;

        // The UI Label to inform the user that the connection is in progress"
        public GameObject progressLabel;

        // The PUN loglevel. 
        public PhotonLogLevel loglevel = PhotonLogLevel.Informational;

        #endregion

        #region Private Variables

        // This client's version number. Users are separated from each other by gameversion.
        private const string GameVersion = "0.1";

        // This is used to make sure that we can quit the game.
        private bool isConnecting;

        #endregion

        #region Monobehaviour Callbacks

        private void Awake()
        {
            // We don't join the lobby. There is no need to join a lobby to get the list of rooms.
            PhotonNetwork.autoJoinLobby = false;

            // All clients in the same room sync their level automaticaly.
            PhotonNetwork.automaticallySyncScene = true;

            // #NotImportant: Force LogLevel
            PhotonNetwork.logLevel = loglevel;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion

        #region Photon.PunBehaviour Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("BattleRobo/Launcher: OnConnectedToMaster() was called by PUN");

            // We don't want to do anything if we are not attempting to join a room. 
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnDisconnectedFromPhoton()
        {
            Debug.LogWarning("BattleRobo/Launcher: OnDisconnectedFromPhoton() was called by PUN");
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log(
                "BattleRobo/Launcher: OnPhotonRandomJoinFailed() was called by PUN");

            // We failed to join a random room, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayersPerRoom}, null);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("BattleRobo/Launcher: OnJoinedRoom() called by PUN");

            // We only load if we are the first player, else we rely on PhotonNetwork.automaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.room.PlayerCount == 1)
            {
                // Load the Room Level. 
                PhotonNetwork.LoadLevel("StartScene");
            }
        }
        
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            Application.Quit();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt to join a random room.
        /// - Esle, connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            // keep track of the will to join a room
            isConnecting = true;

            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            // We check if we are connected or not, we join if we are, else we initiate the connection to the server.
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings(GameVersion);
            }
        }

        public void Quit()
        {
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.LeaveRoom();
            }
        }

        #endregion
    }
}