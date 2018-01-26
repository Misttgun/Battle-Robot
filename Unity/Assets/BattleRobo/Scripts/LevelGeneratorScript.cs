using UnityEngine;

public class LevelGeneratorScript : Photon.PunBehaviour
{
    [SerializeField]
    private GameObject[] prefabsToLoad;

    [SerializeField]
    private int mapSize;

    private static  Vector3[] spawningPointArr;

    void Start()
    {
        Random.InitState(42);

        spawningPointArr = new Vector3[4];


        int spawningPointIndex = 0;

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                var randomNum = Random.Range(1, prefabsToLoad.Length) - 1;

                Instantiate(prefabsToLoad[randomNum],
                    new Vector3(j * 10, prefabsToLoad[randomNum].transform.position.y, i * 10), Quaternion.identity);

                // Boolean defining the spawn points for the player
                // TODO Faire quelque chose de plus robuste
                var topLeftCorner = i == 0 && j == 0;
                var topRightCorner = i == 0 && j == mapSize - 1;
                var bottomLeftCorner = i == mapSize - 1 && j == 0;
                var bottomRightCorner = i == mapSize - 1 && j == mapSize - 1;

                if (!topLeftCorner && !topRightCorner && !bottomLeftCorner && !bottomRightCorner) continue;
                var x = j * 10;
                var y = prefabsToLoad[randomNum].transform.position.y + prefabsToLoad[randomNum].transform.localScale.y;
                var z = i * 10;

                spawningPointArr[spawningPointIndex++] = new Vector3(x, y, z);
            }
        }
    }

    public static Vector3[] getSpawningPoints()
    {
//        foreach (var vec in spawningPointArr)
//        {
//            Debug.LogWarning("Spawning point : (" + vec.x + ", " + vec.y + ", " + vec.z + ")");
//        }
        return spawningPointArr;
    }
}