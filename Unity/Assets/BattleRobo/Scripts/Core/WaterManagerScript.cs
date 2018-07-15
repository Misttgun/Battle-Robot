using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
    public class WaterManagerScript : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            other.GetComponent<HealthScript>().inWater = true;
        }
    }
}