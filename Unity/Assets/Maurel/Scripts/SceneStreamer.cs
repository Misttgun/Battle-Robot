using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maurel.BattleRobo.SceneStreaming
{
    [AddComponentMenu("Scene Streaming/Scene Streamer")]
    public class SceneStreamer : MonoBehaviour
    {
        [Serializable]
        private struct NamedScenes
        {
            public int sceneId;
            public GameObject scenePrefab;
        }

        /// <summary>
        /// Array of NamedScenes which will be converted in a Dictionnary for fast access.
        /// </summary>
        [SerializeField]
        [Tooltip("Add scenes name and scenes id.")]
        private NamedScenes[] scenes;
        
        private static SceneStreamer instance;

        private static object sLock = new object();

        /// <summary>
        /// The Id of the player's current scene.
        /// </summary>
        private int currentSceneId;

        private Dictionary<int, GameObject> sceneList = new Dictionary<int, GameObject>();

        /// <summary>
        /// The names of all loaded scenes.
        /// </summary>
        private HashSet<string> loaded = new HashSet<string>();

        /// <summary>
        /// The names of all scenes near the current scene.
        /// This is used when determining which neighboring scenes to load or unload.
        /// </summary>
        private HashSet<string> near = new HashSet<string>();

        [Tooltip("Tick to log debug info to the Console window.")]
        public bool debug;

        public bool logDebugInfo
        {
            get { return debug && Debug.isDebugBuild; }
        }

        private static SceneStreamer Instance
        {
            get
            {
                lock (sLock)
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType(typeof(SceneStreamer)) as SceneStreamer;
                        if (instance == null)
                        {
                            instance =
                                new GameObject("Scene Loader", typeof(SceneStreamer)).GetComponent<SceneStreamer>();
                        }
                    }
                    return instance;
                }
            }
        }

        public void Awake()
        {
            foreach (var scene in scenes)
            {
                sceneList.Add(scene.sceneId, scene.scenePrefab);
            }
            if (instance)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Sets the current scene, loads it, and manages neighbors. The scene must be in your
        /// project's build settings.
        /// </summary>
        /// <param name="sceneId">Scene Id.</param>
        private void SetCurrent(int sceneId)
        {
            if (sceneId == currentSceneId || sceneId == 0)
            {
                return;
            }

            if (logDebugInfo)
            {
                Debug.Log("Scene Streamer: Setting current scene to Scene" + sceneId + ".");
            }

            LoadCurrentScene(sceneId);
        }

        /// <summary>
        /// Loads a scene as the current scene and manages neighbors, loading closest neighbor.
        /// </summary>
        /// <param name="sceneId">Scene Id.</param>
        private void LoadCurrentScene(int sceneId)
        {
            // First load the current scene:
            currentSceneId = sceneId;
            
            GameObject scenePrefab;
            bool prefabExist = sceneList.TryGetValue(currentSceneId, out scenePrefab);

            if (!prefabExist)
            {
                return;
            }
            
            if (!SceneManager.GetSceneByName(scenePrefab.name).isLoaded)
            {
                SceneManager.LoadSceneAsync(scenePrefab.name, LoadSceneMode.Additive);
                loaded.Add(scenePrefab.name);
            }


            if (logDebugInfo)
            {
                Debug.Log("Scene Streamer: Loading closest neighbors of Scene" + sceneId + ".");
            }

            // Next load neighbors
            LoadNeighbors(sceneId);

            // Finally unload any scenes not in the neighbors list:
            UnloadFarScenes();
        }

        /// <summary>
        /// Loads neighbor scenes within maxNeighborDistance, adding them to the near list.
        /// </summary>
        /// <param name="sceneId">Scene Id.</param>
        private void LoadNeighbors(int sceneId)
        {
            // Clears the near list before loading the neighbors
            near.Clear();
            
            GameObject scenePrefab;
            bool prefabExist = sceneList.TryGetValue(sceneId, out scenePrefab);

            if (!prefabExist)
            {
                return;
            }
            
            NeighborScenes neighborScenes = scenePrefab.GetComponent<NeighborScenes>();

            if (!neighborScenes)
            {
                return;
            }

            foreach (var s in neighborScenes.sceneNames)
            {
                // Add all the neighbors to near list
                near.Add(s);
                
                if (!SceneManager.GetSceneByName(s).isLoaded)
                {
                    SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive);
                    loaded.Add(s);
                }
            }
        }

        /// <summary>
        /// Unloads scenes that are not close neighbors. Assumes the near list has already been populated.
        /// </summary>
        private void UnloadFarScenes()
        {
            HashSet<string> far = new HashSet<string>(loaded);
            far.ExceptWith(near);
            
            // Remove the current scene from far list
            far.Remove("Scene" + currentSceneId);

            if (logDebugInfo && far.Count > 0)
            {
                Debug.Log("Scene Streamer: Unloading scenes away from current Scene" + currentSceneId + ".");
            }
            
            foreach (var sceneName in far)
            {
                if (SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    SceneManager.UnloadSceneAsync(sceneName);
                    loaded.Remove(sceneName);
                }
            }
        }


        /// <summary>
        /// Sets the current scene.
        /// </summary>
        /// <param name="sceneId">Scene Id.</param>
        public static void SetCurrentScene(int sceneId)
        {
            Instance.SetCurrent(sceneId);
        }
    }
}