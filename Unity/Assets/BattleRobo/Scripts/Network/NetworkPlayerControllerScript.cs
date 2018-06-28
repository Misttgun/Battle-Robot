using Photon;

public class NetworkPlayerControllerScript : PunBehaviour
{

    private float health = 100f;

    public float Health
    {
        get { return health; }
    }

    public bool IsDead;
    
    [PunRPC]
    public void TakeDamage(ushort amount)
    {
        health -= amount;

        if (health <= 0)
        {
            health = 0;
            // player is dead
            Die();
        }
    }


    private void Die()
    {
        var photonId = photonView.instantiationId;

        if (photonId == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            //GameManagerScript.Instance.alivePlayerNumber--;
            if (!photonView.isMine)
            {
                return;
            }
            IsDead = true;
            //GameManagerScript.Instance.ShowGameOverScreen("You died... Feels bad man !!");
        }
    }
}