using UnityEngine;
using UnityEngine.UI;

namespace BattleRobo.UI
{
    public class PlayerNameUIScript : MonoBehaviour
    {
        [SerializeField] private Button connectButton;
		[SerializeField] private Button createButton;

        private void Start()
        {
            // Disable the connect button
            connectButton.interactable = false;
        }

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
    }
}