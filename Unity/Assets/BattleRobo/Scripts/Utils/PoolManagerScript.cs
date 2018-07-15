using System.Collections.Generic;
using UnityEngine;

namespace BattleRobo
{
    public class PoolManagerScript : MonoBehaviour
    {
        //mapping of prefab to PoolScript container managing all of its instances
        private static readonly Dictionary<GameObject, PoolScript> Pools = new Dictionary<GameObject, PoolScript>();


        /// <summary>
        /// Called by each PoolScript on its own, this adds it to the dictionary.
        /// </summary>
        public static void Add(PoolScript poolScript)
        {
            //check if the PoolScript does not contain a prefab
            if (poolScript.prefab == null)
            {
                //Debug.LogError("Prefab of poolScript: " + poolScript.gameObject.name + " is empty! Can't add poolScript to Pools Dictionary.");
                return;
            }

            //check if the PoolScript has been added already
            if (Pools.ContainsKey(poolScript.prefab))
            {
                //Debug.LogError("PoolScript with prefab " + poolScript.prefab.name + " has already been added to Pools Dictionary.");
                return;
            }

            //add it to dictionary
            Pools.Add(poolScript.prefab, poolScript);
        }


        /// <summary>
        /// Creates a new PoolScript at runtime. This is being called for prefabs which have not been linked
        /// to a PoolScript in the scene in the editor, but are called via Spawn() nonetheless.
        /// </summary>
        public static void CreatePool(GameObject prefab, int preLoad, bool limit, int maxCount)
        {
            //debug error if poolScript was already added before 
            if (Pools.ContainsKey(prefab))
            {
                //Debug.LogError("PoolScript Manager already contains PoolScript for prefab: " + prefab.name);
                return;
            }

            //create new gameobject which will hold the new PoolScript component
            GameObject newPoolGO = new GameObject("PoolScript " + prefab.name);
            //add PoolScript component to the new gameobject in the scene
            PoolScript newPoolScript = newPoolGO.AddComponent<PoolScript>();
            //assign default parameters
            newPoolScript.prefab = prefab;
            newPoolScript.preLoad = preLoad;
            newPoolScript.limit = limit;
            newPoolScript.maxCount = maxCount;
            //let it initialize itself after assigning variables
            newPoolScript.Awake();
        }


        /// <summary>
        /// Activates a pre-instantiated instance for the prefab passed in, at the desired position.
        /// </summary>
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            //debug a Log entry in case the prefab was not found in a PoolScript
            //this is not critical as then we create a new PoolScript for it at runtime
            if (!Pools.ContainsKey(prefab))
            {
                //Debug.Log("Prefab not found in existing poolScript: " + prefab.name + " New PoolScript has been created.");
                CreatePool(prefab, 0, false, 0);
            }

            //spawn instance in the corresponding PoolScript
            return Pools[prefab].Spawn(position, rotation);
        }


        /// <summary>
        /// Disables a previously spawned instance for later use.
        /// </summary>
        public static void Despawn(GameObject instance, float time = 0f)
        {
            if(time > 0) GetPool(instance).Despawn(instance, time);
            else GetPool(instance).Despawn(instance);
        }


        /// <summary>
        /// Convenience method for quick lookup of a pooled object.
        /// Returns the PoolScript component where the instance has been found in.
        /// </summary>
        public static PoolScript GetPool(GameObject instance)
        {
            //go over Pools and find the instance
            foreach (GameObject prefab in Pools.Keys)
            {
                if (Pools[prefab].active.Contains(instance))
                    return Pools[prefab];
            }

            //the instance could not be found in a PoolScript
            //Debug.LogError("PoolManagerScript couldn't find PoolScript for instance: " + instance.name);
            return null;
        }


        /// <summary>
        /// Despawns all instances of a PoolScript, making them available for later use.
        /// </summary>
        public static void DeactivatePool(GameObject prefab)
        {
            //debug error if PoolScript wasn't already added before
            if (!Pools.ContainsKey(prefab))
            {
                //Debug.LogError("PoolManagerScript couldn't find PoolScript for prefab to deactivate: " + prefab.name);
                return;
            }

            //cache active count
            int count = Pools[prefab].active.Count;
            //loop through each active instance
            for (int i = count - 1; i > 0; i--)
            {
                Pools[prefab].Despawn(Pools[prefab].active[i]);
            }
        }


        /// <summary>
        /// Destroys all despawned instances of all Pools to free up memory.
        /// The parameter 'limitToPreLoad' decides if only instances above the preload
        /// value should be destroyed, to keep a minimum amount of disabled instances
        /// </summary>
        public static void DestroyAllInactive(bool limitToPreLoad)
        {
            foreach (GameObject prefab in Pools.Keys)
                Pools[prefab].DestroyUnused(limitToPreLoad);
        }


        /// <summary>
        /// Destroys the PoolScript for a specific prefab.
        /// Active or inactive instances are not available anymore after calling this.
        /// </summary>
        public static void DestroyPool(GameObject prefab)
        {
            //debug error if PoolScript wasn't already added before
            if (!Pools.ContainsKey(prefab))
            {
                //Debug.LogError("PoolManagerScript couldn't find PoolScript for prefab to destroy: " + prefab.name);
                return;
            }

            //destroy poolScript gameobject including all children. Our game logic doesn't reparent instances,
            //but if you do, you should loop over the active and deactive instances manually to destroy them
            Destroy(Pools[prefab].gameObject);
            //remove key-value pair from dictionary
            Pools.Remove(prefab);
        }


        /// <summary>
        /// Destroys all Pools stored in the manager dictionary.
        /// Active or inactive instances are not available anymore after calling this.
        /// </summary>
        public static void DestroyAllPools()
        {
            //loop over dictionary and destroy every poolScript gameobject
            //see DestroyPool method for further comments
            foreach (GameObject prefab in Pools.Keys)
                DestroyPool(Pools[prefab].gameObject);
        }


        //static variables always keep their values over scene changes
        //so we need to reset them when the game ended or switching scenes
        private void OnDestroy()
        {
            Pools.Clear();
        }
    }
}