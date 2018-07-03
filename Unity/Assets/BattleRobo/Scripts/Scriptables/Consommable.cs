using UnityEngine;

namespace BattleRobo
{
    [CreateAssetMenu(menuName = "Scriptables/Consommable")]
    public class Consommable : ScriptableObject
    {
        public int lifeBonus;
        public int shieldBonus;
        public float castTime;
        public string consommableName;
        public int consommableId;
    }
}