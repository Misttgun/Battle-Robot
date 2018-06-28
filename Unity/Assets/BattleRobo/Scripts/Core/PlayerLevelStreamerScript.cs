using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
    public class PlayerLevelStreamerScript : MonoBehaviour
    {
        public Transform target;
        
        private void Update()
        {
            transform.position = target.position;
        }
    }
}