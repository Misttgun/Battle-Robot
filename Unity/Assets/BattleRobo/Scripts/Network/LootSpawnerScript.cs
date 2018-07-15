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
            var random = Random.Range(0, 100);
            int index = CalculateRandom(random);
            
            if (obj != null)
                return;

            var spawPosition = new Vector3(transform.position.x, transform.position.y + 2.55f, transform.position.z);

            //use the poolmanager to spawn the loot on top of the plateforme
            obj = PoolManagerScript.Spawn(prefabs[index], spawPosition, prefabs[index].gameObject.transform.localRotation);

            var pObject = obj.GetComponent<PlayerObjectScript>();

            pObject.SetLootTrackerIndex(LootTracker.Count);
            LootTracker.Add(pObject);
        }

        public static List<PlayerObjectScript> GetLootTracker()
        {
            return LootTracker;
        }

        private int CalculateRandom(int random)
        {
            int index = 0;

            if (random >= 0 && random <= 39)
            {
                index = 0;
            }
            else if (random >= 40 && random <= 54)
            {
                index = 1;
            }
            else if (random >= 55 && random <= 74)
            {
                index = 2;
            }
            else if (random >= 75 && random <= 84)
            {
                index = 3;
            }
            else if (random >= 85 && random <= 94)
            {
                index = 4;
            }
            else if (random >= 95 && random <= 99)
            {
                index = 5;
            }

            return index;
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