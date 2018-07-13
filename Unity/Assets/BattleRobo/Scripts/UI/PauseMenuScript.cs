using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BattleRobo
{
    public class PauseMenuScript : Photon.PunBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        [SerializeField] private GameObject settingsPanel;
        
        [SerializeField] private GameObject graphicsPanel;

        [SerializeField] private GameObject keysBiddingPanel;
        
        [SerializeField] private GameObject keyEnterPanel;

        [SerializeField] private Text timerText;

        [SerializeField] private Text pauseCounter;

        public void ShowPause()
        {
            pausePanel.SetActive(true);
            settingsPanel.SetActive(false);
            graphicsPanel.SetActive(false);
            keysBiddingPanel.SetActive(false);
            keyEnterPanel.SetActive(false);
        }

        public void ShowSettings()
        {
            pausePanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        public void UpdateTimer(float timer)
        {
            timerText.text = timer.ToString("F0");
        }

        public void UpdatePauseCount(int count)
        {
            pauseCounter.text = count.ToString();
        }

        public void BackToLobby()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(1);
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnDisable()
        {
//            Cursor.lockState = CursorLockMode.Locked;
//            Cursor.visible = false;
        }
    }
}