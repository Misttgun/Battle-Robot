using UnityEngine;

namespace BattleRobo
{
    public class PlayerLevelStreamerScript : MonoBehaviour
    {
        public Transform target;

        private void Start()
        {
            transform.parent = null;
        }

        private void Update()
        {
            transform.position = target.position;
        }
    }
}