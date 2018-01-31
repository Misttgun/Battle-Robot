﻿using UnityEngine;

namespace BattleRobo.Networking
{
    public class LevelGeneratorScript : Photon.PunBehaviour
    {
        [SerializeField] private GameObject[] prefabsToLoad;
        [SerializeField] private Transform level;
        [SerializeField] private int mapSize;

        private void Start()
        {
            Random.InitState(42);

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    var randomNum = Random.Range(1, prefabsToLoad.Length) - 1;

                    var platform = Instantiate(prefabsToLoad[randomNum],
                        new Vector3(j * 10, prefabsToLoad[randomNum].transform.position.y, i * 10),
                        Quaternion.identity);
                    platform.transform.parent = level;
                }
            }
        }
    }
}