using UnityEngine;
using UnityEngine.UI;

namespace BattleRobo.Utilities
{
    public class PlayerNameScript : MonoBehaviour
    {
        #region Private Variables

        [SerializeField]
        private Button connectButton;

        #endregion


        #region MonoBehaviour CallBacks

        private void Start()
        {
            // Disable the connect button
            connectButton.interactable = false;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value)
        {
            connectButton.interactable = !string.IsNullOrEmpty(value.Trim());

            // Force a trailing space string in case value is an empty string, else playerName would not be updated.
            PhotonNetwork.playerName = value + " ";
        }

        #endregion
    }
}