using System.Configuration;
using UnityEngine;

namespace BattleRobo
{
    public class ConsommableScript : MonoBehaviour
    {
        [SerializeField]
        private Consommable consommableStat;

        public int GetId()
        {
            return consommableStat.consommableId;
        }

        public int GetHealth()
        {
            return consommableStat.lifeBonus;
        }

        public int GetShield()
        {
            return consommableStat.shieldBonus;
        }

        public float GetTime()
        {
            return consommableStat.castTime;
        }
    }
}