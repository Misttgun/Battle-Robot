using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
    public class PlayerLevelStreamerScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerGameObject;
        
        // Update is called once per frame
        private void Update()
        {
            transform.position = playerGameObject.transform.position;
        }
    }
}