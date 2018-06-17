﻿using System;
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

		[SerializeField] private Text createPseudo;
		[SerializeField] private Text createPassword;	
		[SerializeField] private Text connectPseudo;
		[SerializeField] private Text connectPassword;
		[SerializeField] private Text error;


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
			string url = "http://51.38.235.234:8080/add_player?pseudo="+createPseudo.text+"&pass="+createPassword.text;

			WWW www = new WWW(url);

			// - wait response
			while (!www.isDone);

			// - player is add successfully
			if (www.responseHeaders ["STATUS"].Contains ("200")) 
			{
				createPanel.SetActive (false);
				Connect ();
			}
			// - can't add player
			else 
				GoToErrorPanel (www.text);
		}

		public void Authenticate()
		{
			string url = "http://51.38.235.234:8080/auth?pseudo="+connectPseudo.text+"&pass="+connectPassword.text;

			WWW www = new WWW(url);

			// - wait response
			while (!www.isDone);

			// - player authenticate successfully
			if (www.responseHeaders ["STATUS"].Contains ("200")) 
				Connect ();

			// - can't add player
			else 
				GoToErrorPanel (www.text);
		}


        public void GoToCreatePanel()
        {
            controlPanel.SetActive(false);
            createPanel.SetActive(true);
        }

        public void GoToErrorPanel(string error_text)
        {
            errorPanel.SetActive(true);
			error.text = error_text;
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