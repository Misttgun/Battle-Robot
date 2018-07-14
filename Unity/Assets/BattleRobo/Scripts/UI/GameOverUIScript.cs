using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BattleRobo
{
    public class GameOverUIScript : Photon.PunBehaviour
    {
        // The game over text
        [SerializeField]
        private Text gameOverText;
        
        // The rank text
        [SerializeField]
        private Text rankText;
        
        // The kill text
        [SerializeField]
        private Text killsText;

        /// <summary>
        /// Updates the game over text
        /// </summary>
        /// <param name="goText"></param>
        /// <param name="rank"></param>
        /// <param name="kills"></param>
        public void UpdateGameOverText(string goText, int rank, int kills)
        {
            gameOverText.text = goText;
            rankText.text = rank.ToString();
            killsText.text = kills.ToString();
        }

        /// <summary>
        /// Return the player to the lobby
        /// </summary>
        public void BackToLobby()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(1);
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}