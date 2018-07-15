using BattleRobo;
using UnityEngine;

public class SpawnGeneratorScript : MonoBehaviour
{
    [SerializeField]
    private LevelGeneratorScript mapGenerator;

    // An array of spawn points
    public Vector3[] spawnPositions;

    // The number of spawn points
    [Tooltip("The number must be a mutliple of 4")]
    [RangeStep(4, 64, 4)]
    public int numSpawnPoints;

    private void Awake()
    {
        spawnPositions = new Vector3[numSpawnPoints];

        GenenrateSpawnPoints();
    }

    private void GenenrateSpawnPoints()
    {
        int numBeforeChange = numSpawnPoints / 4;
        int mapSize = mapGenerator.GetMapMainSize();
        int step = mapSize / numBeforeChange;

        int x = 0, z = 0;

        // The first spawn point is the origin
        spawnPositions[0] = new Vector3(0, 30, 0);

        for (int i = 1; i < numSpawnPoints; i++)
        {
            if (z < mapSize && x == 0)
            {
                z += step;
            }
            else if (z == mapSize && x < mapSize)
            {
                x += step;
            }
            else if (z > 0 && x == mapSize)
            {
                z -= step;
            }
            else if (z == 0 && x > 0)
            {
                x -= step;
            }

            var newX = x - 10;
            var newZ = z - 10;
            
            spawnPositions[i] = new Vector3(Mathf.Clamp(newX, 0, 790), 30, Mathf.Clamp(newZ, 0, 790));
        }
    }
}