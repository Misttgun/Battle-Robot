using UnityEngine;

namespace BattleRobo
{
    public class HealthScript : MonoBehaviour
    {
        [SerializeField]
        private PhotonView playerPhotonView;

        [SerializeField]
        private float damageMultiplicator;
        
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
                SetPlayerStats(playerPhotonView.GetKills(), 0, GameManagerScript.GetInstance().dbTokens[playerPhotonView.owner.ID]);


                //tell all clients that the player is dead
                playerPhotonView.RPC("IsDeadRPC", PhotonTargets.All, playerPhotonView.owner.ID);

                //decrease the number of player alive
                GameManagerScript.alivePlayerNumber--;
            }
            else
            {
                //we didn't die, set health to new value
                playerPhotonView.SetHealth(health);
            }
        }
        
        private void SetPlayerStats(int kills, int win, string token)
        {
            string url = "http://51.38.235.234:8080/update_player?token=" + token + "&kill=" + kills + "&win=" + win;

            // don't wait for response
            WWW www = new WWW(url);
        }
    }
}