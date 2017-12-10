using UnityEngine;

namespace Maurel.BattleRobo.SceneStreaming
{
    [AddComponentMenu("Scene Streaming/Scene Edge")]
    public class SceneCollider : MonoBehaviour
    {
        /// <summary>
        /// The current scene name.
        /// </summary>
        [Tooltip("The id of this scene")] 
        public int currentSceneId;

        public string acceptedTags = "Player";

        /// <summary>
        /// When the player enters this edge (for example coming in from a neighbor),
        /// makes sure to set the current scene to this edge's scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(acceptedTags))
            {
                SceneStreamer.SetCurrentScene(currentSceneId);
            }
        }
    }
}