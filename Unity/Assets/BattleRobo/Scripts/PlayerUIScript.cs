using UnityEngine;
using UnityEngine.UI;

public class PlayerUIScript : MonoBehaviour
{
    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private Slider fuelBar;

    [SerializeField]
    private Text aliveTexT;

    [SerializeField]
    private Text gameOverText;

    [SerializeField]
    private NetworkRoboControllerScript playerScript;
    
    [SerializeField]
    private GameObject gamePanel;
    
    [SerializeField]
    private GameObject gameOverPanel;

    private void Start()
    {        
        UpdateHealth(playerScript.Health);
        UpdateFuel(playerScript.FuelAmount);
        UpdateAliveText(GameManagerScript.Instance.alivePlayerNumber);
        
        gamePanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    private void Update()
    {        
        UpdateHealth(playerScript.Health);
        UpdateFuel(playerScript.FuelAmount);
        UpdateAliveText(GameManagerScript.Instance.alivePlayerNumber);
        
        Debug.LogWarning("IsDead : " + playerScript.IsDead);
        
        gameOverText.text = playerScript.IsDead ? "You died ..." : "You won !!";
        
        if (GameManagerScript.Instance.alivePlayerNumber == 1 || playerScript.IsDead)
        {
            gamePanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }
        
    }

    public void UpdateHealth(float currHealth, float maxHealth = 100f)
    {
        float health = currHealth / maxHealth;
        healthBar.value = health;
    }

    public void UpdateFuel(float fuel)
    {
        fuelBar.value = fuel;
    }

    public void UpdateAliveText(int numberPlayer)
    {
        aliveTexT.text = "Players Alive : " + numberPlayer;
    }
}