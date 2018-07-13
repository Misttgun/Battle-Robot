using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BattleRobo
{
    public class PauseMenuScript : Photon.PunBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        [SerializeField] private GameObject settingsPanel;

        [SerializeField] private Text timerText;

        [SerializeField] private Text pauseCounter;

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
    }
}