using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRobo
{
    public class MainMenuScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject loadingScreenPanel;

        [SerializeField]
        private Text playerNumbersLabel;

        public GameObject loadingMessageLabel;

        public GameObject readyMessageLabel;

        public Text redyCountDownLabel;

        public Button cancelButton;

        [SerializeField]
        private GameObject lobbyPanel;

        [SerializeField]
        private GameObject leaderboardPanel;

        [SerializeField]
        private GameObject settingsPanel;

        [SerializeField]
        private List<GameObject> leaderboardRows;

        [SerializeField]
        private Color playerColor;

        private void Start()
        {
            lobbyPanel.SetActive(true);
            loadingScreenPanel.SetActive(false);
        }

        public void ShowLoadingScreen()
        {
            lobbyPanel.SetActive(false);
            loadingScreenPanel.SetActive(true);
        }

        public void ShowMainMenu()
        {
            lobbyPanel.SetActive(true);
            loadingScreenPanel.SetActive(false);
        }

        public void ShowLeaderboard()
        {
            leaderboardPanel.SetActive(true);
            lobbyPanel.SetActive(false);
            settingsPanel.SetActive(false);
            FillLeaderboard();
        }

        public void ShowLobby()
        {
            leaderboardPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            settingsPanel.SetActive(false);
        }

        public void ShowSettings()
        {
            leaderboardPanel.SetActive(false);
            lobbyPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        public void SetPlayerNumbersLabel(int number)
        {
            playerNumbersLabel.text = number.ToString();
        }

        private void FillLeaderboard()
        {
            int status;
            string response;
            bool hasFoundPlayer = false;

            DatabaseRequester.GetInstance().SyncQuery("/leaderboard?token=" + PlayerInfoScript.GetInstance().GetDBToken(), out status, out response);

            // - leaderboard is loaded successfully
            if (status == 200)
            {
                var csv_text = response;
                var rows = csv_text.Split('\n');

                for (int i = 0; i < rows.Length; i++)
                {
                    var row = rows[i].Split(',');

                    if (row.Length == 5 && i < leaderboardRows.Count)
                    {
                        GameObject leaderboardRow = leaderboardRows[i];

                        Text rankText = (Text)leaderboardRow.transform.GetChild(0).GetComponent("Text");
                        Text playerText = (Text) leaderboardRow.transform.GetChild(1).GetComponent("Text");
                        Text killText = (Text) leaderboardRow.transform.GetChild(2).GetComponent("Text");
                        Text winText = (Text) leaderboardRow.transform.GetChild(3).GetComponent("Text");
                        Text playedText = (Text)leaderboardRow.transform.GetChild(4).GetComponent("Text");
                        Text scoreText = (Text) leaderboardRow.transform.GetChild(5).GetComponent("Text");

                        playerText.text = row[0];
                        killText.text = row[1];
                        winText.text = row[2];
                        playedText.text = row[3];
                        scoreText.text = row[4];
                        
                        if (System.String.Equals(row[0], DatabaseRequester.GetInstance().GetPlayerPseudo(), System.StringComparison.OrdinalIgnoreCase))
                        {
                            rankText.color = playerColor;
                            playerText.color = playerColor;
                            winText.color = playerColor;
                            killText.color = playerColor;
                            playedText.color = playerColor;
                            scoreText.color = playerColor;
                            hasFoundPlayer = true;
                        }
                    }
                }

                if (hasFoundPlayer)
                {
                    leaderboardRows[10].SetActive(false);
                }

                else
                {
                    var row = rows[10].Split(',');
                    leaderboardRows[10].SetActive(true);
                    GameObject leaderboardRow = leaderboardRows[10];

                    Text rankText = (Text)leaderboardRow.transform.GetChild(0).GetComponent("Text");
                    Text playerText = (Text)leaderboardRow.transform.GetChild(1).GetComponent("Text");
                    Text killText = (Text)leaderboardRow.transform.GetChild(2).GetComponent("Text");
                    Text winText = (Text)leaderboardRow.transform.GetChild(3).GetComponent("Text");
                    Text playedText = (Text)leaderboardRow.transform.GetChild(4).GetComponent("Text");
                    Text scoreText = (Text)leaderboardRow.transform.GetChild(5).GetComponent("Text");

                    rankText.text = row[0];
                    playerText.text = row[1];
                    killText.text = row[2];
                    winText.text = row[3];
                    playedText.text = row[4];
                    scoreText.text = row[5];

                    rankText.color = playerColor;
                    playerText.color = playerColor;
                    winText.color = playerColor;
                    killText.color = playerColor;
                    playedText.color = playerColor;
                    scoreText.color = playerColor;
                    hasFoundPlayer = true;
                }
            }

            else
            {
                // TODO ERROR MESSAGE
            }
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}