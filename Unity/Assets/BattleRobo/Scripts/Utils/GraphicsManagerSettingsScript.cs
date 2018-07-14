using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRobo
{
    public class GraphicsManagerSettingsScript : MonoBehaviour
    {
        [SerializeField]
        private Toggle fullscreenToggle;

        [SerializeField]
        private Dropdown resolutionDropdown;

        [SerializeField]
        private Dropdown antialiasingDropdown;

        [SerializeField]
        private Dropdown vSyncDropdown;

        [SerializeField]
        private Dropdown textureDropdown;

        [SerializeField]
        private Dropdown graphicsDropdown;
        
        [SerializeField]
        private GameObject settingsPanel;

        private Resolution[] resolutions;

        private void Start()
        {
            LoadSettings();

            // mettre en plein écran si le paramètre enregistré est true
            if (Convert.ToBoolean(PlayerPrefs.GetString("FullScreen", "true")))
            {
                Screen.fullScreen = true;
            }
        }

        private void OnEnable()
        {
            antialiasingDropdown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
            vSyncDropdown.onValueChanged.AddListener(delegate { OnVSyncChange(); });
            textureDropdown.onValueChanged.AddListener(delegate { OnTextureQChange(); });
            graphicsDropdown.onValueChanged.AddListener(delegate { OnGraphicsQChange(); });
            
            resolutions = Screen.resolutions;
            
            if (resolutionDropdown.options.Count == 0)
            {
                foreach (Resolution resolution in resolutions)
                {
                    resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
                }
            }
        }

        /// <summary>
        /// Toggle full screen , and applied on save button click
        /// </summary>
        public void FullScreenMod()
        {
            Screen.fullScreen = fullscreenToggle.isOn;
        }

        private void OnAntialiasingChange()
        {
            QualitySettings.antiAliasing = (int) Mathf.Pow(2, antialiasingDropdown.value);
        }

        private void OnVSyncChange()
        {
            QualitySettings.vSyncCount = vSyncDropdown.value;
        }

        private void OnTextureQChange()
        {
            QualitySettings.masterTextureLimit = textureDropdown.value;
        }

        private void OnGraphicsQChange()
        {
            QualitySettings.SetQualityLevel(graphicsDropdown.value);
        }

        public void ApplyButtonClick()
        {
            Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
            //apply after click 
            SaveSettings();
        }

        // save settings 
        private void SaveSettings()
        {
            PlayerPrefs.SetString("FullScreen", Screen.fullScreen.ToString());
            PlayerPrefs.SetString("Resolution", resolutionDropdown.value.ToString());
            PlayerPrefs.SetString("VSync", vSyncDropdown.value.ToString());
            PlayerPrefs.SetString("Antialiasing", antialiasingDropdown.value.ToString());
            PlayerPrefs.SetString("Texture", textureDropdown.value.ToString());
            PlayerPrefs.SetString("Graphics", graphicsDropdown.value.ToString());

            PlayerPrefs.Save();

            LoadSettings();
        }

        // load settings
        private void LoadSettings()
        {
            fullscreenToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetString("FullScreen", "false"));
            resolutionDropdown.value = Convert.ToInt32(PlayerPrefs.GetString("Resolution", "4"));
            vSyncDropdown.value = Convert.ToInt32(PlayerPrefs.GetString("VSync", "0"));
            antialiasingDropdown.value = Convert.ToInt32(PlayerPrefs.GetString("Antialiasing", "0"));
            textureDropdown.value = Convert.ToInt32(PlayerPrefs.GetString("Texture", "0"));
            graphicsDropdown.value = Convert.ToInt32(PlayerPrefs.GetString("Graphics", "5"));

            QualitySettings.SetQualityLevel(graphicsDropdown.value);
            resolutionDropdown.RefreshShownValue();
            vSyncDropdown.RefreshShownValue();
            antialiasingDropdown.RefreshShownValue();
            textureDropdown.RefreshShownValue();
        }
        
        public void ShowSettings()
        {
            gameObject.SetActive(false);
            settingsPanel.SetActive(true);
        }
    }
}