using UnityEngine;

namespace BattleRobo
{
	public class LootTriggerScript : MonoBehaviour
	{		
		[SerializeField]
		private WeaponScript weaponScript;

        [SerializeField]
        private ConsommableScript consommableScript;
		
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
	
		/*
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

            if (weaponScript)
			    weaponScript.enabled = true;
            if (consommableScript)
                consommableScript.enabled = true;

			meshRenderer.enabled = true;
			weaponLight.SetActive(true);
		}
		*/

		private void OnTriggerStay(Collider other)
		{
			if (count > 1)
				return;
			
			if (playerObjectScript.IsAvailable() == false)
			{
				return;
			}
			
			//lootScript.enabled = false;
            if (weaponScript)
			    weaponScript.enabled = true;
            if (consommableScript)
                consommableScript.enabled = true;
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
            if (weaponScript)
			    weaponScript.enabled = false;
            if (consommableScript)
                consommableScript.enabled = false;
			
			if (playerObjectScript.IsAvailable() == false)
			{
				return;
			}
			
			meshRenderer.enabled = false;
			weaponLight.SetActive(false);
		}
	}
}