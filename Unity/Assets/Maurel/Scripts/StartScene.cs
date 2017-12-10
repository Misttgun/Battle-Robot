using UnityEngine;

namespace Maurel.BattleRobo.SceneStreaming
{

	[AddComponentMenu("Scene Streamer/Set Start Scene")]
	public class StartScene : MonoBehaviour {

		/// <summary>
		/// The Id of the scene to load at Start.
		/// </summary>
		private int startSceneId;
		
		/// <summary>
		/// The number of the scenes
		/// </summary>
		[SerializeField] private int numberOfScenes;

		public void Start() 
		{
			//var rng = Random.Range(1, numberOfScenes);
			//startSceneId = "Scene" + rng;
			startSceneId = 1;
			SceneStreamer.SetCurrentScene(startSceneId);
			Destroy(this);
		}
	}
}
