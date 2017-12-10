using UnityEngine;

namespace Maurel.BattleRobo.SceneStreaming
{
	[AddComponentMenu("Scene Streaming/Neighbor Scenes")]
	public class NeighborScenes : MonoBehaviour
	{
		/// <summary>
		/// The scene's neighbors.
		/// </summary>
		[Tooltip("This scene's neighbors")]
		public string[] sceneNames;
	}
}
