using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorScript : Photon.PunBehaviour
{
    [SerializeField]
    private GameObject[] prefabsToLoad;

    [SerializeField]
    private int mapSize;

    private Vector3[] spawningPointArr;
    
    void Start()
    {
        Random.InitState(42);
       
        spawningPointArr = new Vector3[4];

        int randomNum,
            spawningPointIndex = 0;
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                randomNum = Random.Range(1, prefabsToLoad.Length) - 1;
                
                // - création des games object pour le spawn
                bool topLeftCorner = (i == 0 && j == 0),
                     topRightCorner = (i == 0 && j == (mapSize - 1)),
                     bottomLeftCorner = (i == (mapSize - 1) && j == 0),
                     bottomRightCorner = (i == (mapSize - 1) && j == (mapSize - 1));
                     
                Instantiate(prefabsToLoad[randomNum],
                    new Vector3(j * 10, prefabsToLoad[randomNum].transform.position.y, i * 10), Quaternion.identity);

                if (topLeftCorner || topRightCorner || bottomLeftCorner || bottomRightCorner)
                {
                    var x = j * 10;
                    var y = prefabsToLoad[randomNum].transform.position.y + prefabsToLoad[randomNum].transform.localScale.y;
                    var z = i * 10;
                        
                    spawningPointArr[spawningPointIndex++] = new Vector3(x, y, z);
                }
            }
        }
    }

    public Vector3[] getSpawningPoints()
    {
        return spawningPointArr;
    }
}