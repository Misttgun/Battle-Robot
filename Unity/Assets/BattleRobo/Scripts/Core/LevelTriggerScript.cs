using UnityEngine;

namespace BattleRobo
{
    public class LevelTriggerScript : MonoBehaviour
    {

        [SerializeField]
        private GameObject levelPrefab;

        private GameObject spawnedLevel;

        private void OnTriggerEnter(Collider other)
        {
            Debug.LogWarning("Je suis dans le trigger");
            spawnedLevel = PoolManagerScript.Spawn(levelPrefab, transform.position, Quaternion.identity);
        }
        
        private void OnTriggerExit(Collider other)
        {
            Debug.LogWarning("Je suis plus dans le trigger");
            PoolManagerScript.Despawn(spawnedLevel);
        }
    }
}