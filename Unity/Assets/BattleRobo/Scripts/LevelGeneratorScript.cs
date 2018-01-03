using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorScript : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefabsToLoad;

    [SerializeField]
    private int mapSize;

    void Start()
    {
        Random.InitState(42);

        int randomNum;
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                randomNum = Random.Range(1, prefabsToLoad.Length) - 1;
                Instantiate(prefabsToLoad[randomNum],
                    new Vector3(j * 10, prefabsToLoad[randomNum].transform.position.y, i * 10), Quaternion.identity);
            }
        }
        
        
    }
}