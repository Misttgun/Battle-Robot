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

//		/// <summary>
//        /// The max number of neighbors to load out from the current scene.
//        /// </summary>
//        [Tooltip("Max number of neighbors to load out from the current scene.")]
//        public int maxNeighborDistance = 8;
//
//        /// <summary>
//        /// A failsafe in case loading hangs. After this many seconds, the SceneStreamer
//        /// will stop waiting for the scene to load.
//        /// </summary>
//        [Tooltip("(Failsafe) If scene doesn't load after this many seconds, stop waiting.")]
//        public float maxLoadWaitTime = 10f;
//
//        [System.Serializable]
//        public class StringEvent : UnityEvent<string> { }
//	    
//        [System.Serializable]
//        public class StringAsyncEvent : UnityEvent<string, AsyncOperation> { }
//
//        public StringAsyncEvent onLoading = new StringAsyncEvent();
//
//        public StringEvent onLoaded = new StringEvent();
//
//        [Tooltip("Tick to log debug info to the Console window.")]
//        public bool debug;
//
//        public bool logDebugInfo { get { return debug && Debug.isDebugBuild; } }
//
//        /// <summary>
//        /// The name of the player's current scene.
//        /// </summary>
//        private string m_currentSceneName;
//
//        /// <summary>
//        /// The names of all loaded scenes.
//        /// </summary>
//        private HashSet<string> loaded = new HashSet<string>();
//
//        /// <summary>
//        /// The names of all scenes that are in the process of being loaded.
//        /// </summary>
//        private HashSet<string> loading = new HashSet<string>();
//
//        /// <summary>
//        /// The names of all scenes within maxNeighborDistance of the current scene.
//        /// This is used when determining which neighboring scenes to load or unload.
//        /// </summary>
//        private HashSet<string> near = new HashSet<string>();
//
//        private static object sLock = new object();
//
//        private static SceneStreamer instance;
//
//        private static SceneStreamer Instance
//        {
//            get
//            {
//                lock (sLock)
//                {
//                    if (instance == null)
//                    {
//                        instance = FindObjectOfType(typeof(SceneStreamer)) as SceneStreamer;
//                        if (instance == null)
//                        {
//                            instance = new GameObject("Scene Loader", typeof(SceneStreamer)).GetComponent<SceneStreamer>();
//                        }
//                    }
//                    return instance;
//                }
//            }
//        }
//
//        public void Awake()
//        {
//            if (instance)
//            {
//                Destroy(this);
//            }
//            else
//            {
//                instance = this;
//                DontDestroyOnLoad(gameObject);
//            }
//        }
//
//        /// <summary>
//        /// Sets the current scene, loads it, and manages neighbors. The scene must be in your
//        /// project's build settings.
//        /// </summary>
//        /// <param name="sceneId">Scene name.</param>
//        public void SetCurrent(string sceneId)
//        {
//            if (string.IsNullOrEmpty(sceneId) || string.Equals(sceneId, m_currentSceneName))
//            {
//                return;
//            }
//            
//            if (logDebugInfo)
//            {
//                Debug.Log("Scene Streamer: Setting current scene to " + sceneId + ".");
//            }
//            
//            StartCoroutine(LoadCurrentScene(sceneId));
//        }
//
//        /// <summary>
//        /// Loads a scene as the current scene and manages neighbors, loading scenes
//        /// within maxNeighborDistance and unloading scenes beyond it.
//        /// </summary>
//        /// <returns>The current scene.</returns>
//        /// <param name="sceneId">Scene name.</param>
//        private IEnumerator LoadCurrentScene(string sceneId)
//        {
//            // First load the current scene:
//            m_currentSceneName = sceneId;
//            
//            if (!IsLoaded(m_currentSceneName))
//            {
//                Load(sceneId);
//            }
//            
//            float failsafeTime = Time.realtimeSinceStartup + maxLoadWaitTime;
//            
//            while ((loading.Count > 0) && (Time.realtimeSinceStartup < failsafeTime))
//            {
//                yield return null;
//            }
//            
//            if (Time.realtimeSinceStartup >= failsafeTime && Debug.isDebugBuild)
//            {
//                Debug.LogWarning("Scene Streamer: Timed out waiting to load " + sceneId + ".");
//            }
//
//            // Next load neighbors up to maxNeighborDistance, keeping track
//            // of them in the near list:
//            if (logDebugInfo)
//            {
//                Debug.Log("Scene Streamer: Loading " + maxNeighborDistance + " closest neighbors of " + sceneId + ".");
//            }
//            
//            near.Clear();
//            LoadNeighbors(sceneId, 0);
//            failsafeTime = Time.realtimeSinceStartup + maxLoadWaitTime;
//            
//            while ((loading.Count > 0) && (Time.realtimeSinceStartup < failsafeTime))
//            {
//                yield return null;
//            }
//
//            if (Time.realtimeSinceStartup >= failsafeTime && Debug.isDebugBuild)
//            {
//                Debug.LogWarning("Scene Streamer: Timed out waiting to load neighbors of " + sceneId + ".");
//            }
//
//            // Finally unload any scenes not in the near list:
//            UnloadFarScenes();
//        }
//
//        /// <summary>
//        /// Loads neighbor scenes within maxNeighborDistance, adding them to the near list.
//        /// </summary>
//        /// <param name="sceneId">Scene name.</param>
//        /// <param name="distance">Distance.</param>
//        private void LoadNeighbors(string sceneId, int distance)
//        {
//            if (near.Contains(sceneId) || distance >= maxNeighborDistance)
//            {
//                return;
//            }
//            
//            near.Add(sceneId);
//            
//            GameObject scene = GameObject.Find(sceneId);
//            NeighborScenes neighborScenes = (scene) ? scene.GetComponent<NeighborScenes>() : null;
//            
//            if (!neighborScenes)
//            {
//                return;
//            }
//            
//            foreach (var s in neighborScenes.sceneNames)
//            {
//                Load(s, LoadNeighbors, distance + 1);
//            }
//        }
//
//        /// <summary>
//        /// Determines whether a scene is loaded.
//        /// </summary>
//        /// <returns><c>true</c> if loaded; otherwise, <c>false</c>.</returns>
//        /// <param name="sceneId">Scene name.</param>
//        public bool IsLoaded(string sceneId)
//        {
//            return loaded.Contains(sceneId);
//        }
//
//        /// <summary>
//        /// Loads a scene.
//        /// </summary>
//        /// <param name="sceneId">Scene name.</param>
//        public void Load(string sceneId)
//        {
//            Load(m_currentSceneName, null, 0);
//        }
//
//        private delegate void InternalLoadedHandler(string sceneId, int distance);
//
//        /// <summary>
//        /// Loads a scene and calls an internal delegate when done. The delegate is
//        /// used by the LoadNeighbors() method.
//        /// </summary>
//        /// <param name="sceneId">Scene name.</param>
//        /// <param name="loadedHandler">Loaded handler.</param>
//        /// <param name="distance">Distance from the current scene.</param>
//        private void Load(string sceneId, InternalLoadedHandler loadedHandler, int distance)
//        {
//            if (IsLoaded(sceneId))
//            {
//                if (loadedHandler != null)
//                {
//                    loadedHandler(sceneId, distance);
//                }
//                return;
//            }
//            
//            loading.Add(sceneId);
//
//            if (logDebugInfo && distance > 0)
//            {
//                Debug.Log("Scene Streamer: Loading " + sceneId + ".");
//            }
//
//            StartCoroutine(LoadAdditiveAsync(sceneId, loadedHandler, distance));
//        }
//
//        /// <summary>
//        /// (Unity Pro) Runs Application.LoadLevelAdditiveAsync() and calls FinishLoad() when done.
//        /// </summary>
//        /// <param name="sceneId">Scene name.</param>
//        /// <param name="loadedHandler">Loaded handler.</param>
//        /// <param name="distance">Distance.</param>
//        private IEnumerator LoadAdditiveAsync(string sceneId, InternalLoadedHandler loadedHandler, int distance)
//        {
//            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
//            
//            onLoading.Invoke(sceneId, asyncOperation);
//            
//            yield return asyncOperation;
//            
//            FinishLoad(sceneId, loadedHandler, distance);
//        }
//
//        /// <summary>
//        /// (Unity) Runs Application.LoadLevelAdditive() and calls FinishLoad() when done.
//        /// This coroutine waits two frames to wait for the load to complete.
//        /// </summary>
//        /// <returns>The additive.</returns>
//        /// <param name="sceneId">Scene name.</param>
//        /// <param name="loadedHandler">Loaded handler.</param>
//        /// <param name="distance">Distance.</param>
//        private IEnumerator LoadAdditive(string sceneId, InternalLoadedHandler loadedHandler, int distance)
//        {
//            SceneManager.LoadScene(sceneId, LoadSceneMode.Additive);
//            
//            onLoading.Invoke(sceneId, null);
//            
//            yield return new WaitForEndOfFrame();
//            yield return new WaitForEndOfFrame();
//            
//            FinishLoad(sceneId, loadedHandler, distance);
//        }
//
//        /// <summary>
//        /// Called when a level is done loading. Updates the loaded and loading lists, and 
//        /// calls the loaded handler.
//        /// </summary>
//        /// <param name="sceneId">Scene name.</param>
//        /// <param name="loadedHandler">Loaded handler.</param>
//        /// <param name="distance">Distance.</param>
//        private void FinishLoad(string sceneId, InternalLoadedHandler loadedHandler, int distance)
//        {
//            GameObject scene = GameObject.Find(sceneId);
//            
//            if (scene == null && Debug.isDebugBuild)
//            {
//                Debug.LogWarning("Scene Streamer: Can't find loaded scene named '" + sceneId + "'.");
//            }
//
//            loading.Remove(sceneId);
//            loaded.Add(sceneId);
//            onLoaded.Invoke(sceneId);
//
//            if (loadedHandler != null)
//            {
//                loadedHandler(sceneId, distance);
//            }
//        }
//
//        /// <summary>
//        /// Unloads scenes beyond maxNeighborDistance. Assumes the near list has already been populated.
//        /// </summary>
//        private void UnloadFarScenes()
//        {
//            HashSet<string> far = new HashSet<string>(loaded);
//            far.ExceptWith(near);
//
//            if (logDebugInfo && far.Count > 0)
//            {
//                Debug.Log("Scene Streamer: Unloading scenes more than " + maxNeighborDistance + " away from current scene " + m_currentSceneName + ".");
//            }
//            foreach (var sceneId in far)
//            {
//                Unload(sceneId);
//            }
//        }
//
//        /// <summary>
//        /// Unloads a scene.
//        /// </summary>
//        /// <param name="sceneId">Scene name.</param>
//        public void Unload(string sceneId)
//        {
//            if (logDebugInfo)
//            {
//                Debug.Log("Scene Streamer: Unloading scene " + sceneId + ".");
//            }
//            
//            Destroy(GameObject.Find(sceneId));
//            
//            loaded.Remove(sceneId);
//            
//            SceneManager.UnloadSceneAsync(sceneId);
//        }
//
//        /// <summary>
//        /// Sets the current scene.
//        /// </summary>
//        /// <param name="sceneId">Scene name.</param>
//        public static void SetCurrentScene(string sceneId)
//        {
//            Instance.SetCurrent(sceneId);
//        }
//
//        /// <summary>
//        /// Determines if a scene is loaded.
//        /// </summary>
//        /// <returns><c>true</c> if loaded; otherwise, <c>false</c>.</returns>
//        /// <param name="sceneId">Scene name.</param>
//        public static bool IsSceneLoaded(string sceneId)
//        {
//            return Instance.IsLoaded(sceneId);
//        }
//
//        /// <summary>
//        /// Loads a scene.
//        /// </summary>
//        /// <param name="sceneId">Scene name.</param>
//        public static void LoadScene(string sceneId)
//        {
//            Instance.Load(sceneId);
//        }
//
//        /// <summary>
//        /// Unloads a scene.
//        /// </summary>
//        /// <param name="sceneId">Scene name.</param>
//        public static void UnloadScene(string sceneId)
//        {
//            Instance.Unload(sceneId);
//        }
    }
}