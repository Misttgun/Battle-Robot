using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
    public class PlayerLevelStreamerScript : MonoBehaviour
    {
        private Quaternion rotation;

        private void Awake()
        {
            rotation = transform.rotation;
        }

        private void LateUpdate()
        {
            transform.rotation = rotation;
        }
    }
}