using BattleRobo.Core;
using UnityEngine;
using UnityEngine.UI;
using BattleRobo.Networking;

namespace BattleRobo.UI
{
    public class PlayerUIScript : MonoBehaviour
    {
        // The health bar
        [SerializeField] private Slider healthBar;

        // The fuel bar
        [SerializeField] private Slider fuelBar;

        // The number of player alive text
        [SerializeField] private Text aliveTexT;

        // A reference to the network player controller
        [SerializeField] private NetworkPlayerControllerScript networkPlayerScript;
        
        // A reference to the network player controller
        [SerializeField] private PlayerControllerScript playerScript;

        // A reference to the game UI
        [SerializeField] private GameObject gamePanel;

        private void Start()
        {
            UpdateHealth(networkPlayerScript.Health);
            UpdateFuel(playerScript.FuelAmount);
            UpdateAliveText(GameManagerScript.Instance.alivePlayerNumber);

            gamePanel.SetActive(true);
        }

        private void Update()
        {
            UpdateHealth(networkPlayerScript.Health);
            UpdateFuel(playerScript.FuelAmount);
            UpdateAliveText(GameManagerScript.Instance.alivePlayerNumber);

            if (GameManagerScript.Instance.alivePlayerNumber == 1 || networkPlayerScript.IsDead)
            {
                gamePanel.SetActive(false);
            }
        }

        /// <summary>
        /// Updates the player health
        /// </summary>
        /// <param name="currHealth"></param>
        /// <param name="maxHealth"></param>
        private void UpdateHealth(float currHealth, float maxHealth = 100f)
        {
            var health = currHealth / maxHealth;
            healthBar.value = health;
        }

        /// <summary>
        /// Updates the player fuel
        /// </summary>
        /// <param name="fuel"></param>
        private void UpdateFuel(float fuel)
        {
            fuelBar.value = fuel;
        }

        /// <summary>
        /// Updates the number of player alive in the game
        /// </summary>
        /// <param name="numberPlayer"></param>
        private void UpdateAliveText(int numberPlayer)
        {
            aliveTexT.text = "Players Alive : " + numberPlayer;
        }
    }
}