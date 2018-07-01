using UnityEngine;

namespace BattleRobo
{
    public class LevelTriggerScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject levelPrefab;

        private GameObject spawnedLevel;

        //variable qui compte le nombre de trigger en collision avec le level
        private int count;

        private void OnTriggerEnter(Collider other)
        {
            ++count;
            //Debug.LogWarning("OncollisionEnter: " + gameObject.name + " - Pos: " + transform.position +  " - Count: " + count);
            if (count > 1)
                return;

            spawnedLevel = PoolManagerScript.Spawn(levelPrefab, transform.position, Quaternion.identity);
        }


        private void OnTriggerExit(Collider other)
        {
            --count;
            //Debug.LogWarning("OncollisionExit: " + gameObject.name + " - Pos: " + transform.position +  " - Count: " + count);
            if (count > 0)
                return;

            PoolManagerScript.Despawn(spawnedLevel);
        }
    }
}