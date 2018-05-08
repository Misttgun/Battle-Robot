﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
    public class WaterManagerScript : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.LogWarning("Dans l'eau");
            other.GetComponent<RoboController>().inWater = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            Debug.LogWarning("Dans l'eau");
            other.GetComponent<RoboController>().inWater = false;
        }
    }
}