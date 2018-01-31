using BattleRobo.Core;
using UnityEngine;
using BattleRobo.UI;

namespace BattleRobo.Networking
{
    public class GameManagerScript : Photon.PunBehaviour
    {
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private GameOverUIScript gameOverUiScript;
        [SerializeField] private GameObject gameCamera;
        public Transform[] spawnPoints;

        public static GameManagerScript Instance;
        public int alivePlayerNumber;

        private void Start()
        {
            if (Instance == null)
                Instance = this;

            alivePlayerNumber = PhotonNetwork.room.PlayerCount;
            gameOverUI.SetActive(false);
            gameCamera.SetActive(false);
            
            PhotonNetwork.Instantiate("RobotWheelNetwork", Vector3.zero, Quaternion.identity, 0);
        }

        private void Update()
        {
            if (alivePlayerNumber == 1)
            {
                ShowGameOverScreen("You won !! Let's go baby !!");
            }
        }

        public void ShowGameOverScreen(string goText)
        {
            gameCamera.SetActive(true);
            gameOverUI.SetActive(true);
            gameOverUiScript.UpdateGameOverText(goText);
        }
    }
}