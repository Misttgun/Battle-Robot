using UnityEngine;

namespace BattleRobo
{
    [CreateAssetMenu(menuName = "Scriptables/Gun")]
    public class Gun : ScriptableObject
    {
        public bool twoHanded;
        public int magazineSize;
        public int damage;
        public float range;
        public float fireRate;
        public string gunName;
    }
}