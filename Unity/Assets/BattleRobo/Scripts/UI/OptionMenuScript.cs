using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRobo
{
    public class OptionMenuScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject graphicsPanel;

        [SerializeField]
        private GameObject controlsPanel;

        [SerializeField]
        private GameObject settingsPanel;

        [SerializeField]
        private Slider sound;
        
        [SerializeField]
        private Slider mouseSensitivity;
        
        [SerializeField]
        private Text displayMouse;
        
        [SerializeField]
        private Text displaySound;

        public void Start()
        {
            AudioListener.volume = Convert.ToSingle(PlayerPrefs.GetString("Sound", "0.5"));
            
            //Adds a listener to the sliders and invokes a method when the value changes.
            sound.onValueChanged.AddListener(delegate { AudioValueChangeCheck(); });
            mouseSensitivity.onValueChanged.AddListener(delegate { MouseValueChangeCheck(); });
        }

        private void OnEnable()
        {
            sound.value = Convert.ToSingle(PlayerPrefs.GetString("Sound", "0.5"));
            displaySound.text = sound.value.ToString(CultureInfo.InvariantCulture);
            
            mouseSensitivity.value = Convert.ToSingle(PlayerPrefs.GetString("Sensitivity", "5.0"));
            displayMouse.text = mouseSensitivity.value.ToString(CultureInfo.InvariantCulture);
        }

        public void AudioValueChangeCheck()
        {
            AudioListener.volume = sound.value;
            PlayerPrefs.SetString("Sound", sound.value.ToString(CultureInfo.InvariantCulture));
            displaySound.text = sound.value.ToString(CultureInfo.InvariantCulture);
        }
        
        public void MouseValueChangeCheck()
        {
            PlayerPrefs.SetString("Sensitivity", mouseSensitivity.value.ToString(CultureInfo.InvariantCulture));
            displayMouse.text = mouseSensitivity.value.ToString(CultureInfo.InvariantCulture);
        }

        public void ShowControls()
        {
            controlsPanel.SetActive(true);
            settingsPanel.SetActive(false);
            graphicsPanel.SetActive(false);
        }

        public void ShowGraphics()
        {
            graphicsPanel.SetActive(true);
            controlsPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }

        public void ShowSettings()
        {
            graphicsPanel.SetActive(false);
            controlsPanel.SetActive(false);
            settingsPanel.SetActive(true);
            
            PlayerPrefs.Save();
        }
    }
}