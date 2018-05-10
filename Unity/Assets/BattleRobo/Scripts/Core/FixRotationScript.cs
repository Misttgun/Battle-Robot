using UnityEngine;

namespace BattleRobo
{
    public class FixRotationScript : MonoBehaviour
    {
        private Quaternion rotation;

        private void FixedUpdate()
        {
            rotation = transform.rotation;
        }

        private void LateUpdate()
        {
            transform.rotation = rotation;
        }
    }
}