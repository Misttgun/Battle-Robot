using UnityEngine;

namespace Maurel.Scripts
{
    public class Launcher : MonoBehaviour
    {
        #region Public Variables

        #endregion

        #region Private Variables

        /// <summary>
        /// This client's version number. Users are separated from each other by gameversion.
        /// </summary>
        private string _gameVersion = "0.1";

        #endregion

        #region Monobehaviour Callbacks

        private void Awake()
        {
            // #Critical
            // We don't join the lobby. There is no need to join a lobby to get the list of rooms.
            PhotonNetwork.autoJoinLobby = false;

            // #Critical
            // This makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automaticaly
            PhotonNetwork.automaticallySyncScene = true;
        }

        private void Start()
        {
            Connect();
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
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.connected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        #endregion
    }
}