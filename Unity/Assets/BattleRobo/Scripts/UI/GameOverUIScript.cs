using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BattleRobo.UI
{
    public class GameOverUIScript : PunBehaviour
    {
        // The game over text
        [SerializeField] private Text gameOverText;

        /// <summary>
        /// Updates the game over text
        /// </summary>
        /// <param name="goText"></param>
        public void UpdateGameOverText(string goText)
        {
            gameOverText.text = goText;
        }
        
        /// <summary>
        /// Return the player to the lobby
        /// </summary>
        public void BackToLobby()
        {
            if(!photonView.isMine)
                return;
            
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(1);
        }
    }
}
