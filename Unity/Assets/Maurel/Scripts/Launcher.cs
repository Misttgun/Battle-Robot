using UnityEngine;

namespace Maurel.BattleRobo.Networking
{
    public class Launcher : Photon.PunBehaviour
    {
        #region Public Variables

        /// <summary>
        /// The maximum number of players per room.
        /// </summary>   
        [Tooltip(
            "The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        public byte maxPlayersPerRoom = 20;

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        public GameObject controlPanel;

        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        public GameObject progressLabel;

        /// <summary>
        /// The PUN loglevel. 
        /// </summary>
        public PhotonLogLevel loglevel = PhotonLogLevel.Informational;

        #endregion

        #region Private Variables

        /// <summary>
        /// This client's version number. Users are separated from each other by gameversion.
        /// </summary>
        private const string GameVersion = "0.1";

        /// <summary>
        /// This is used to make sure that we can quit the game.
        /// </summary>
        bool isConnecting;

        #endregion

        #region Monobehaviour Callbacks

        private void Awake()
        {
            // #Critical: We don't join the lobby. There is no need to join a lobby to get the list of rooms.
            PhotonNetwork.autoJoinLobby = false;

            // #Critical: All clients in the same room sync their level automaticaly.
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

            // we don't want to do anything if we are not attempting to join a room. 
            if (isConnecting)
            {
                // #Critical: We try to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()  
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
                "BattleRobo/Launcher: OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.");

            // #Critical: We failed to join a random room, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayersPerRoom}, null);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("BattleRobo/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

            // #Critical: We only load if we are the first player, else we rely on PhotonNetwork.automaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.room.PlayerCount == 1)
            {
                Debug.Log("We load the Start Scene ");


                // #Critical: Load the Room Level. 
                PhotonNetwork.LoadLevel("StartScene");
            }
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

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.connected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings(GameVersion);
            }
        }

        #endregion
    }
}