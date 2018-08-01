using System;
using BattleRobo;
using UnityEngine;

public class SpawnGeneratorScript : MonoBehaviour
{
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
        const float mapSize = 410f;
        float step = mapSize / numBeforeChange;

        float x = 0, z = 0;

        // The first spawn point is the origin
        spawnPositions[0] = new Vector3(-20f, 40.5f, -20f);

        for (int i = 1; i < numSpawnPoints; i++)
        {
            if (z < mapSize && Math.Abs(x) < 0.0001f)
            {
                z += step;
            }
            else if (Math.Abs(z - mapSize) < 0.0001f && x < mapSize)
            {
                x += step;
            }
            else if (z > 0 && Math.Abs(x - mapSize) < 0.0001f)
            {
                z -= step;
            }
            else if (Math.Abs(z) < 0.0001f && x > 0)
            {
                x -= step;
            }

            var newX = x;
            var newZ = z;

            if (Math.Abs(newX) < 0.0001f)
            {
                newX = -20f;
            }
            else if (Math.Abs(newZ) < 0.0001f)
            {
                newZ = -20f;
            }


            spawnPositions[i] = new Vector3(Mathf.Clamp(newX, -20, 790), 40.5f, Mathf.Clamp(newZ, -20, 790));
        }
    }
}