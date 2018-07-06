using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ManagorSettings : MonoBehaviour
{
	
	[SerializeField] private Toggle fullscreenToggle;
	[SerializeField] private Dropdown resolutionDropdown;
	[SerializeField] private Dropdown antialiasingDropdown;
	[SerializeField] private Dropdown vSyncDropdown;
	[SerializeField] private Dropdown textureDropdown;
	[SerializeField] private Dropdown graphicsDropdown;
	[SerializeField] private Slider sensitivityMouseSlider; //,sfxVolumeSlider;
	//[SerializeField] private AudioMixer gameMixerSound;
	[SerializeField] private Text displayS;
		
	private Resolution[] resolutions;
	private GameSettings gameSettings;
	private bool fullscreenP;

	private void Start()
	{
		gameSettings = new GameSettings();
		loadSettings();
		if (gameSettings.fullscreen) // mettre en plein écran si le paramètre enregistré est true
		{
			Screen.fullScreen = true;
		}

	}

	private void OnEnable()
	{
		
		resolutionDropdown.onValueChanged.AddListener(delegate{ OnResolutionChange(); });
		antialiasingDropdown.onValueChanged.AddListener(delegate{ OnAntialiasingChange(); });
		vSyncDropdown.onValueChanged.AddListener(delegate{ OnVSyncChange(); });
		textureDropdown.onValueChanged.AddListener(delegate{ OnTextureQChange(); });
		graphicsDropdown.onValueChanged.AddListener(delegate{ OnGraphicsQChange(); });
		
		resolutions = Screen.resolutions;
		foreach (Resolution resolution in resolutions)
		{
			resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
		}
	}

	/// <summary>
	/// Toggle full screen , and applied on save button click
	/// </summary>
	public void fullScreenMod(bool fullScreen)
	{
		fullscreenP = fullScreen;
		
	}

	private void OnResolutionChange()
	{
		
	}

	private void OnAntialiasingChange()
	{
		QualitySettings.antiAliasing = gameSettings.antialiasing = (int)Mathf.Pow(2, antialiasingDropdown.value);
	}

	private void OnVSyncChange()
	{
		QualitySettings.vSyncCount = gameSettings.vSync = vSyncDropdown.value;
	}

	private void OnTextureQChange()
	{
		QualitySettings.masterTextureLimit = gameSettings.texture = textureDropdown.value;
	}

	private void OnGraphicsQChange()
	{
		gameSettings.graphics = graphicsDropdown.value;
		QualitySettings.SetQualityLevel(graphicsDropdown.value);
	}
	/*
	public void setVolume(float volumeValue)
	{
		gameMixerSound.SetFloat("volume", volumeValue);

		if (gameSettings!=null)
		{
			gameSettings.sfxvolume = volumeValue;
		}
		
		
	}*/
	
	/// <summary>
	/// Save all change and apply
	/// </summary>
	public void applyButtonClick()
	{
		Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height,Screen.fullScreen);
		gameSettings.resolution = resolutionDropdown.value;
		gameSettings.fullscreen = Screen.fullScreen = fullscreenP; //apply after click 
		saveSettings();
		
	}
	
	
	public void mouseSensitivityChange(float sensitivity)
	{
		float s;
		s=gameSettings.sensitivity = sensitivity;
		displayS.text = s.ToString();
	}
	
	// save settings 
	private void saveSettings()
	{
		string jsonData = JsonUtility.ToJson(gameSettings, true);
		File.WriteAllText(Application.persistentDataPath+ "/gameSettings.json", jsonData); // Users/USER/AppData/LocalLow/ESGI../..
	}
	
	// load settings
	private void loadSettings()
	{
		gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath+"/gameSettings.json"));

		fullscreenToggle.isOn = gameSettings.fullscreen;
		resolutionDropdown.value = gameSettings.resolution;
		vSyncDropdown.value = gameSettings.vSync;
		antialiasingDropdown.value = gameSettings.antialiasing;
		textureDropdown.value = gameSettings.texture;
		graphicsDropdown.value = gameSettings.graphics;
		sensitivityMouseSlider.value = gameSettings.sensitivity;
		QualitySettings.SetQualityLevel(gameSettings.graphics);
		
		//gameMixerSound.SetFloat("volume", sfxVolumeSlider.value);
		//sfxVolumeSlider.value = gameSettings.sfxvolume;
		
		resolutionDropdown.RefreshShownValue();
		vSyncDropdown.RefreshShownValue();
		antialiasingDropdown.RefreshShownValue();
		textureDropdown.RefreshShownValue();

	}

	public float getSensitivity()
	{
		return gameSettings.sensitivity;
	}
}
