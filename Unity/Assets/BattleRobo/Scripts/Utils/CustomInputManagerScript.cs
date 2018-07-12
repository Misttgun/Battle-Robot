using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
	public class CustomInputManagerScript : MonoBehaviour
	{
		public static readonly Dictionary<string, KeyCode> keyBind = new Dictionary<string, KeyCode>(15);

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
			
			keyBind.Add("Up", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up", "Z")));
			keyBind.Add("Down", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down", "S")));
			keyBind.Add("Right", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", "D")));
			keyBind.Add("Left", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", "Q")));
			keyBind.Add("Fly", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Fly", "Space")));
			keyBind.Add("Drop", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Drop", "G")));
			keyBind.Add("Loot", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Loot", "F")));
			keyBind.Add("Slot1", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Slot1", "Alpha1")));
			keyBind.Add("Slot2", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Slot2", "Alpha2")));
			keyBind.Add("Slot3", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Slot3", "Alpha3")));
			keyBind.Add("Slot4", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Slot4", "Alpha4")));
			keyBind.Add("Slot5", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Slot5", "Alpha5")));
		}
			
	}

}

