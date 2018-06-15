using UnityEngine;

namespace BattleRobo
{
	public class LootScript : MonoBehaviour
	{
		/// <summary>
		/// Reference to the local object (script) that spawned this powerup.
		/// </summary>
		//[HideInInspector]
		public LootSpawnerScript spawner;


		private void Start()
		{
			//cross-referencing this gameobject in the spawner
			spawner.obj = gameObject;
		}
	}
}