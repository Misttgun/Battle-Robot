using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
    public class PlayerLevelStreamerScript : MonoBehaviour
    {
        //private Quaternion rotation;

        public Transform target;

        private void Awake()
        {
            //rotation = transform.rotation;
        }

        private void Update()
        {
            transform.position = target.position;
        }
//
//        private void LateUpdate()
//        {
//            transform.rotation = rotation;
//        }
    }
}