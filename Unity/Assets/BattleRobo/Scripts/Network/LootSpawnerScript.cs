using System.Collections.Generic;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

namespace BattleRobo
{
    public class LootSpawnerScript : MonoBehaviour
    {
        /// <summary>
        /// Prefab to sync the instantiation for.
        /// </summary>
        public List<GameObject> prefabs;


        /// <summary>
        /// Reference to the spawned prefab gameobject instance in the scene.
        /// </summary>
        //[HideInInspector]
        public GameObject obj;

        private static List<PlayerObjectScript> LootTracker;

        private void Start()
        {
            // instantiate the LootTracker only one time !
            if (LootTracker == null)
                LootTracker = new List<PlayerObjectScript>();

            SpawnLoot();
        }

        /// <summary>
        /// Instantiates the object in the scene using PoolManagerScript functionality.
        /// </summary>
        private void SpawnLoot()
        {
            Debug.Log("COUNT PREFABS : " + prefabs.Count);
            int index = Random.Range(0, prefabs.Count);
            if (obj != null)
                return;

            var spawPosition = new Vector3(transform.position.x, transform.position.y + 2.55f, transform.position.z);
            var spawnRotation = transform.rotation * Quaternion.Euler(0, 0, 90);

            //use the poolmanager to spawn the loot on top of the plateforme
            obj = PoolManagerScript.Spawn(prefabs[index], spawPosition, spawnRotation);

            var pObject = obj.GetComponent<PlayerObjectScript>();

            pObject.SetLootTrackerIndex(LootTracker.Count);
            LootTracker.Add(pObject);
        }

        public static List<PlayerObjectScript> GetLootTracker()
        {
            return LootTracker;
        }

        /// <summary>
        /// Called by the spawned object to destroy itself on this managing component.
        /// For example when it has been collected by players.
        /// </summary>
        public void Destroy()
        {
            //despawn object and clear references
            PoolManagerScript.Despawn(obj);
            obj = null;
        }
    }
}