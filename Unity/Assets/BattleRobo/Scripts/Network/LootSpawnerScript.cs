using System.Collections;
using System.Collections.Generic;
using Photon;
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

        private static List<GameObject> LootTracker;

        private void Start()
        {
            // instantiate the LootTracker only one time !
            if (LootTracker == null)
                LootTracker = new List<GameObject>();
            
            SpawnLoot();
        }

        /// <summary>
        /// Instantiates the object in the scene using PoolManagerScript functionality.
        /// </summary>
        private void SpawnLoot()
        {
            int index = Random.Range(0, prefabs.Count);
            if (obj != null)
                return;
            
            var spawPosition = new Vector3(transform.position.x, transform.position.y + 3.5f, transform.position.z);
            var spawnRotation = transform.rotation * Quaternion.Euler(0, 0, 90);

            //use the poolmanager to spawn the loot on top of the plateforme
            obj = PoolManagerScript.Spawn(prefabs[index], spawPosition, spawnRotation);

            //set the reference on the instantiated object for cross-referencing
            obj.GetComponent<LootScript>().spawner = this;
            obj.GetComponent<PlayerObject>().SetLootTrackerIndex(LootTracker.Count);
            LootTracker.Add(obj);
        }

        public static List<GameObject> GetLootTracker()
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