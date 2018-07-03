using UnityEngine;
using Photon;

namespace BattleRobo
{
    public class PlayerObjectScript : PunBehaviour
    {
        //TODO Should be move in item script
        [SerializeField] private int id;
        [SerializeField] private int maxStackSize;
        [SerializeField] private Sprite itemSprite;

        /// <summary>
        /// - position on the map
        /// </summary>
        private Transform position;

        /// <summary>
        /// - playerPhotonView used for RPC
        /// </summary>
        private PhotonView myPhotonView;

        /// <summary>
        /// - Update Model to ItemScript for more genericity
        /// </summary>
        [SerializeField]
        private WeaponScript weaponScript;

        /// <summary>
        /// - Update Model to ItemScript for more genericity
        /// </summary>
        [SerializeField]
        private ConsommableScript consommableScript;

        [SerializeField]
        private LootTriggerScript lootTriggerScript;

        /// <summary>
        /// - unique id, which is the position in the LootSpawner::LootTracker list
        /// </summary>
        private int lootTrackerIndex;

        /// <summary>
        /// - use to handle concurrency looting
        /// </summary>
        private bool isAvailable;

        /// <summary>
        /// - true if there is already a TakeObject RPC pending on this object 
        /// for the player photonView
        /// </summary>
        private bool isLooting;

        private void Start()
        {
            isAvailable = true;
            isLooting = false;
        }

        public void Hide()
        {
            lootTriggerScript.DespawnLoot();
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Drop(Vector3 pos)
        {
            isAvailable = true;
            Show();
            transform.position = pos;
        }

        public WeaponScript GetWeapon()
        {
            return weaponScript;
        }

        public ConsommableScript GetConsommable()
        {
            return consommableScript;
        }

        public Sprite GetSprite()
        {
            return itemSprite;
        }

        public void SetLootTrackerIndex(int newId)
        {
            lootTrackerIndex = newId;
        }

        public int GetLootTrackerIndex()
        {
            return lootTrackerIndex;
        }

        public int GetId()
        {
            return id;
        }

        public bool IsAvailable()
        {
            return isAvailable;
        }

        public bool IsLooting()
        {
            return isLooting;
        }

        public void SetLooting(bool looting)
        {
            isLooting = looting;
        }

        public void SetAvailable(bool available)
        {
            isAvailable = available;

        }

        public int GetMaxStack()
        {
            return maxStackSize;
        }
    }
}