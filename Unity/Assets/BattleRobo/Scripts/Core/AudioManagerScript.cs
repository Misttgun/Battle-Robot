using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleRobo
{
    /// <summary>
    /// Handles playback of background music, 2D and 3D one-shot clips during the game.
    /// Makes use of the PoolManager for activating 3D AudioSources at desired world positions.
    /// </summary>
    public class AudioManagerScript : MonoBehaviour
    {
        //reference to this script instance
        private static AudioManagerScript instance;

        /// <summary>
        /// AudioSource for playing back lengthy music clips.
        /// </summary>
        public AudioSource musicSource;

        /// <summary>
        /// AudioSource for playing back one-shot 2D clips.
        /// </summary>
        public AudioSource audioSource;

        /// <summary>
        /// Array for storing background music clips, so they can be
        /// referenced in PlayMusic() by passing in their index value.
        /// </summary>
        public AudioClip[] musicClips;

        // Sets the instance reference, if not set already,
        // and keeps listening to scene changes.
        private void Awake()
        {
            if (instance != null)
                return;

            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Returns a reference to this script instance.
        /// </summary>
        public static AudioManagerScript GetInstance()
        {
            return instance;
        }

        // Stop playing music after switching scenes. To keep playing
        // music in the new scene, this requires calling PlayMusic() again.
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            musicSource.Stop();
        }

        /// <summary>
        /// Play sound clip in 2D on the background audio source.
        /// There can only be one music clip playing at the same time.
        /// Only plays music if the player enabled it in the settings.
        /// </summary>
        public static void PlayMusic(int index)
        {
            instance.musicSource.clip = instance.musicClips[index];

            //user settings could have disabled the audio source
            if (instance.musicSource.enabled)
                instance.musicSource.Play();
        }

        /// <summary>
        /// Play sound clip passed in in 2D space.
        /// </summary>
        public static void Play2D(AudioClip clip)
        {
            instance.audioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Play sound clip passed in in 3D space.
        /// </summary>
        public static void Play3D(AudioSource source, AudioClip clip, float pitch = 1f)
        {
            //cancel execution if clip wasn't set
            if (clip == null) return;

            //assign properties, play clip
            source.clip = clip;
            source.pitch = pitch;
            source.Play();
        }
    }
}