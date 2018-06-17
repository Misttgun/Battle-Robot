using System;
using System.Collections.Generic;
using UnityEngine;
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

        public void Create()
        {
            string url = "http://51.38.235.234:8080/add_player?pseudo=Kas&pass=password";

            WWW www = new WWW(url);
            
            // - wait response
            while (!www.isDone);
            
            Debug.Log(" Test : " + (String)www.text + " " + www.responseHeaders["STATUS"]);
        }

        public void GoToCreatePanel()
        {
            controlPanel.SetActive(false);
            createPanel.SetActive(true);
        }

        public void GoToErrorPanel(string error)
        {
            errorPanel.SetActive(true);
        }

        public void GoToConnectPanel()
        {
            createPanel.SetActive(false);
            controlPanel.SetActive(true);
        }

        public void AuthentificationFailed()
        {
            controlPanel.SetActive(false);
            GoToErrorPanel("Authentification failed");
        }

        public void UsernameExist()
        {
            createPanel.SetActive(false);
            GoToErrorPanel("Username already exist");
        }
    }
}