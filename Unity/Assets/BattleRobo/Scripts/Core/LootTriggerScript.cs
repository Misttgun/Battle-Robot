using UnityEngine;

namespace BattleRobo
{
	public class LootTriggerScript : MonoBehaviour
	{		
		[SerializeField]
		private WeaponScript weaponScript;
		
		[SerializeField]
		private PlayerObjectScript playerObjectScript;

		[SerializeField]
		private MeshRenderer meshRenderer;
		
		//[SerializeField]
		//private LootScript lootScript;

		[SerializeField]
		private GameObject weaponLight;
		
		//variable qui compte le nombre de trigger en collision avec le loot
		private int count;
		
		private void Start()
		{
			count = 0;
		}

		private void OnTriggerEnter(Collider other)
		{
			++count;
			if (count > 1)
				return;
			
			if (playerObjectScript.IsAvailable() == false)
			{
				return;
			}
			
			//lootScript.enabled = false;
			weaponScript.enabled = true;
			meshRenderer.enabled = true;
			weaponLight.SetActive(true);
		}
        
		private void OnTriggerExit(Collider other)
		{
			--count;
			if (count > 0)
				return;
			
			DespawnLoot();
		}

		public void DespawnLoot()
		{
			//lootScript.enabled = false;
			weaponScript.enabled = false;			
			
			if (playerObjectScript.IsAvailable() == false)
			{
				return;
			}
			
			meshRenderer.enabled = false;
			weaponLight.SetActive(false);
		}
	}
}