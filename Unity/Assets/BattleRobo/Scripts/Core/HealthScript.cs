using UnityEngine;

namespace BattleRobo
{
    public class HealthScript : MonoBehaviour
    {
        [SerializeField]
        private PhotonView playerPhotonView;

        [SerializeField]
        private float damageMultiplicator;
        
        //safe zone variables
        [HideInInspector]
        public bool inStorm;
        private const float waitingTime = 1f;
        private float timer;

        //water variables
        [HideInInspector]
        public bool inWater;

        private void Update()
        {
            //take storm and water damage only on the master client
            if (!PhotonNetwork.isMasterClient) 
                return;
            
            timer += Time.deltaTime;

            //apply damage to player in the storm
            if (inStorm)
            {
                if (timer > waitingTime)
                {
                    TakeDamage(StormManagerScript.GetInstance().stormDmg);
                    timer = 0f;
                }
            }

            //apply damage to player in the water
            if (inWater)
            {
                Debug.Log("IN WATER, TAKE DAMAGE");
                //insta death when the player touches the water
                TakeDamage(300);
            }
        }

        public void ShowDamageIndicator(Vector3 shooterPos)
        {
            playerPhotonView.RPC("DamageIndicatorRPC", PhotonTargets.AllViaServer, shooterPos);
        }

        /// <summary>
        /// Server only: calculate damage to be taken by the Player,
        /// triggers kills increase and workflow on death.
        /// </summary>
        public void TakeDamage(int hitPoint, int killerID = -1)
        {
            if (!PhotonNetwork.isMasterClient)
                return;

            int damage = (int)(hitPoint * damageMultiplicator);

            //store network variables temporary
            int health = playerPhotonView.GetHealth();
            int shield = playerPhotonView.GetShield();

            //reduce shield on hit
            if (shield > 0 && killerID != -1)
            {
                var shieldDamage = shield - damage;
                
                if (shieldDamage < 0)
                {
                    playerPhotonView.SetShield(0);
                    playerPhotonView.SetHealth(health + shieldDamage);
                }

                else
                    playerPhotonView.SetShield(shieldDamage);
                
                return;
            }

            //if player is already dead, but we can still shoot him... don't ask me why
            if (health <= 0)
            {
                return;
            }

            //substract health by damage
            //locally for now, to only have one update later on
            health -= damage;

            //the player is dead
            if (health <= 0)
            {
                //if we took damage from another player
                if (killerID != -1)
                {
                    //get killer and increase kills for that player
                    playerPhotonView.RPC("UpdateKillsRPC", PhotonTargets.MasterClient, killerID);
                }

                //set the player current rank
                playerPhotonView.SetRank(GameManagerScript.alivePlayerNumber);

                // set dead player stats
                DatabaseRequester.SetPlayerStat(playerPhotonView.GetKills(), 0, GameManagerScript.GetInstance().dbTokens[playerPhotonView.owner.ID]);

                //tell all clients that the player is dead
                playerPhotonView.RPC("IsDeadRPC", PhotonTargets.All, playerPhotonView.owner.ID);
            }
            else
            {
                //we didn't die, set health to new value
                playerPhotonView.SetHealth(health);
            }
        }
    }
}