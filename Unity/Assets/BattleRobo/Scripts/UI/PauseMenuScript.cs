using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BattleRobo
{
    public class PauseMenuScript : Photon.PunBehaviour
    {
        [SerializeField]
        private GameObject pausePanel;
        
        [SerializeField]
        private GameObject settingsPanel;

        public void ShowPause()
        {
            pausePanel.SetActive(true);
            settingsPanel.SetActive(false);
        }

        public void ShowSettings()
        {
            pausePanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        public void Resume()
        {
            pausePanel.SetActive(false);
            settingsPanel.SetActive(false);
        }
        
        /// <summary>
        /// Return the player to the lobby
        /// </summary>
        public void BackToLobby()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(1);
        }
    }
}