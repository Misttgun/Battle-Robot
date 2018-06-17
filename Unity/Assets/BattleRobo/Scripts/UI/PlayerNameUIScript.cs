using UnityEngine;
using UnityEngine.UI;

namespace BattleRobo.UI
{
    public class PlayerNameUIScript : MonoBehaviour
    {
        [SerializeField] private Button connectButton;
		[SerializeField] private Button createButton;

		[SerializeField] private Text connectPseudo;
		[SerializeField] private InputField connectPassword;
		[SerializeField] private Text createPseudo;
		[SerializeField] private InputField createPassword;

        private void Start()
        {
            // Disable the connect button
            connectButton.interactable = false;
			createButton.interactable = false;
        }

        /// <summary>
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value)
        {
			connectButton.interactable = UpdateConnectButtonInteractibility ();

            // Force a trailing space string in case value is an empty string, else playerName would not be updated.
            PhotonNetwork.playerName = value + " ";
        }

		public void SetPlayerPassword(string value)
		{
			connectButton.interactable = UpdateConnectButtonInteractibility ();
		}

		public void SetNewPlayerName(string value)
		{
			createButton.interactable = UpdateCreateButtonInteractibility ();

			// Force a trailing space string in case value is an empty string, else playerName would not be updated.
			PhotonNetwork.playerName = value + " ";
		}

		public void SetNewPlayerPassword(string value)
		{
			createButton.interactable = UpdateCreateButtonInteractibility ();
		}

		public bool UpdateConnectButtonInteractibility()
		{
			string pseudo = connectPseudo.text;
			string password = connectPassword.text;

			return !string.IsNullOrEmpty (pseudo.Trim ()) && !string.IsNullOrEmpty (password.Trim ());
		}

		public bool UpdateCreateButtonInteractibility()
		{
			string pseudo = createPseudo.text;
			string password = createPassword.text;

			return !string.IsNullOrEmpty (pseudo.Trim ()) && !string.IsNullOrEmpty (password.Trim ());
		}
    }
}