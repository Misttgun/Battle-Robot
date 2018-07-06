using BattleRobo;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindingsSettings : MonoBehaviour {

	[SerializeField]
	private Text up, down, right, left, fly, loot, drop, slot1, slot2, slot3,slot4,slot5,savedText;
	
	private GameObject currentKey;

	[SerializeField] private GameObject enterKeyLayer;
	//[SerializeField] private GameObject keyBdS;
	
	private void Start()
	{
		up.text = CustomInputManager.keyBind["Up"].ToString();
		down.text = CustomInputManager.keyBind["Down"].ToString();
		right.text = CustomInputManager.keyBind["Right"].ToString();
		left.text = CustomInputManager.keyBind["Left"].ToString();
		fly.text = CustomInputManager.keyBind["Fly"].ToString();

		slot1.text = CustomInputManager.keyBind["Slot1"].ToString();
		slot2.text = CustomInputManager.keyBind["Slot2"].ToString();
		slot3.text = CustomInputManager.keyBind["Slot3"].ToString();
		slot4.text = CustomInputManager.keyBind["Slot4"].ToString();
		slot5.text = CustomInputManager.keyBind["Slot5"].ToString();
		loot.text = CustomInputManager.keyBind["Loot"].ToString();
		drop.text = CustomInputManager.keyBind["Drop"].ToString();
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (currentKey != null)
		{
			Event keyEvent = Event.current;
			if (keyEvent.isKey)
			{
				CustomInputManager.keyBind[currentKey.name] = keyEvent.keyCode; // changer la touche séléctionée par la prochaine touche entrée
				currentKey.transform.GetChild(0).GetComponent<Text>().text = keyEvent.keyCode.ToString(); // remplacer le text du bouton sélectionné par la touche entrée
				currentKey = null;
				enterKeyLayer.SetActive(false);
			}
		}
		
	}

	/// <summary>
	/// Change the key selected
	/// </summary>
	/// <returns></returns>
	public void ChangeKey(GameObject keyToChange)
	{
		enterKeyLayer.SetActive(true); // layer lors de la selection d'une touche 
		currentKey = keyToChange;
	}

	/// <summary>
	/// Save Keybindings in Dictionary 
	/// </summary>
	/// <returns></returns>
	public void SaveKeys()
	{
		savedText.enabled = true;
		foreach (var key in CustomInputManager.keyBind)
		{
			PlayerPrefs.SetString(key.Key, key.Value.ToString());
		}
		
		PlayerPrefs.Save();
	}




}
