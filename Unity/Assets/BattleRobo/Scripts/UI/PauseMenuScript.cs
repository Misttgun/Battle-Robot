using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRobo
{
    public class PauseMenuScript : MonoBehaviour
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
    }
}