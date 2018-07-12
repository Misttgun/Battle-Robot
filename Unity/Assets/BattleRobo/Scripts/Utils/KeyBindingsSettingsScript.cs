using BattleRobo;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRobo
{
    public class KeyBindingsSettingsScript : MonoBehaviour
    {
        [SerializeField]
        private Text up, down, right, left, fly, loot, drop, slot1, slot2, slot3, slot4, slot5;

        private GameObject currentKey;

        [SerializeField]
        private GameObject enterKeyLayer;
        
        [SerializeField]
        private GameObject settingsPanel;

        private void Start()
        {
            LoadKeys();
        }

        private void OnGUI()
        {
            if (currentKey != null)
            {
                Event keyEvent = Event.current;
                if (keyEvent.isKey)
                {
                    // changer la touche séléctionée par la prochaine touche entrée
                    CustomInputManagerScript.keyBind[currentKey.name] = keyEvent.keyCode; 
                    // remplacer le text du bouton sélectionné par la touche entrée
                    currentKey.transform.GetChild(0).GetComponent<Text>().text = keyEvent.keyCode.ToString(); 
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
            //savedText.enabled = true;
            foreach (var key in CustomInputManagerScript.keyBind)
            {
                PlayerPrefs.SetString(key.Key, key.Value.ToString());
            }

            PlayerPrefs.Save();
        }

        public void LoadKeys()
        {
            up.text = CustomInputManagerScript.keyBind["Up"].ToString();
            down.text = CustomInputManagerScript.keyBind["Down"].ToString();
            right.text = CustomInputManagerScript.keyBind["Right"].ToString();
            left.text = CustomInputManagerScript.keyBind["Left"].ToString();
            fly.text = CustomInputManagerScript.keyBind["Fly"].ToString();
            slot1.text = CustomInputManagerScript.keyBind["Slot1"].ToString();
            slot2.text = CustomInputManagerScript.keyBind["Slot2"].ToString();
            slot3.text = CustomInputManagerScript.keyBind["Slot3"].ToString();
            slot4.text = CustomInputManagerScript.keyBind["Slot4"].ToString();
            slot5.text = CustomInputManagerScript.keyBind["Slot5"].ToString();
            loot.text = CustomInputManagerScript.keyBind["Loot"].ToString();
            drop.text = CustomInputManagerScript.keyBind["Drop"].ToString();
        }
        
        public void ShowSettings()
        {
            gameObject.SetActive(false);
            settingsPanel.SetActive(true);
        }
    }
}