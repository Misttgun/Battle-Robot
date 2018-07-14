﻿using UnityEngine;
using UnityEngine.UI;

namespace BattleRobo.Networking
{
    public class LauncherScript : Photon.PunBehaviour
    {
        // The Ui Panel to let the user enter name, connect and play
        public GameObject controlPanel;

        // The UI Label to inform the user that the connection is in progress
        public GameObject progressLabel;

        // The UI Panl to let the user create an account
        public GameObject createPanel;

        // The UI error Pannel
        public GameObject errorPanel;

        [SerializeField]
        private Text createPseudo;

        [SerializeField]
        private InputField createPassword;

        [SerializeField]
        private Text connectPseudo;

        [SerializeField]
        private InputField connectPassword;

        [SerializeField]
        private Text error;

        [SerializeField]
        private PlayerInfoScript playerInfo;

        // - save previous panel for the back button
        private GameObject lastPanel;


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
            lastPanel = controlPanel;
        }

        public override void OnJoinedLobby()
        {
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

        public void Create()
        {
            int status;
            string response;
            string query = "/add_player?pseudo=" + createPseudo.text + "&pass=" + createPassword.text;

            DatabaseRequester.GetInstance().SyncQuery(query, out status, out response);

            // - player is add successfully
            if (status == 200)
            {
                createPanel.SetActive(false);
                playerInfo.SetDBToken(response);
                Connect();
                DatabaseRequester.GetInstance().PingServer();
                DatabaseRequester.GetInstance().SetPseudo(createPseudo.text);
            }

            // - can't add player
            else
            {
                lastPanel = createPanel;
                GoToErrorPanel(response);
            }
        }

        public void Authenticate()
        {
            int status;
            string response;
            string query = "/auth?pseudo=" + connectPseudo.text + "&pass=" + connectPassword.text;

            DatabaseRequester.GetInstance().SyncQuery(query, out status, out response);
            
            // - player authenticate successfully
            if (status == 200)
            {
                Connect();
                playerInfo.SetDBToken(response);
                DatabaseRequester.GetInstance().PingServer();
                DatabaseRequester.GetInstance().SetPseudo(connectPseudo.text);
            }
            // - can't add player
            else
            {
                lastPanel = controlPanel;
                GoToErrorPanel(response);
            }
        }


        public void GoToErrorPanel(string error_text)
        {
            errorPanel.SetActive(true);
            error.text = error_text;
        }

        public void GoToCreatePanel()
        {
            controlPanel.SetActive(false);
            createPanel.SetActive(true);
        }

        public void GoToLastPanel()
        {
            createPanel.SetActive(false);
            controlPanel.SetActive(false);
            errorPanel.SetActive(false);
            lastPanel.SetActive(true);
            lastPanel = controlPanel;
        }
    }
}