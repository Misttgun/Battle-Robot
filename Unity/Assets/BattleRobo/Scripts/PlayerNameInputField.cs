using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace BattleRobo.Utilities
{
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Variables

        [SerializeField] 
        private Button connectButton;

        // Store the PlayerPref Key to avoid typos
        private static string playerNamePrefKey = "PlayerName";

        #endregion


        #region MonoBehaviour CallBacks

        private void Start()
        {
            string defaultName = "";
            InputField inputField = GetComponent<InputField>();
            if (inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    inputField.text = defaultName;
                }
            }

            PhotonNetwork.playerName = defaultName;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value)
        {
            connectButton.interactable = !value.Trim().IsNullOrEmpty();
            
            // Force a trailing space string in case value is an empty string, else playerName would not be updated.
            PhotonNetwork.playerName = value + " ";
            
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion
    }
}