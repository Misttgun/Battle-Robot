using System;
using System.Collections;
using System.Collections.Generic;
using BattleRobo;
using UnityEngine;
using Photon;

namespace BattleRobo
{
    public class PlayerObjectScript : PunBehaviour
    {
        // - Should be move in item script
        [SerializeField] private int id;
        [SerializeField] private int maxStackSize;
        [SerializeField] private Sprite itemSprite;


        // <summary>
        /// - position on the map
        /// </summary>
        private Transform position;

        // <summary>
        /// - playerPhotonView used for RPC
        /// </summary>
        private PhotonView myPhotonView;

        // <summary>
        /// - Update Model to ItemScript for more genericity
        /// </summary>
        private WeaponScript weaponScript;

        // <summary>
        /// - unique id, which is the position in the LootSpawner::LootTracker list
        /// </summary>
        private int lootTrackerIndex;

        // <summary>
        /// - use to handle concurrency looting
        /// </summary>
        private bool isAvailable;

        // <summary>
        /// - true if there is already a TakeObject RPC pending on this object 
        /// for the player photonView
        /// </summary>
        private bool isLooting;

        public void Start()
        {
            isAvailable = true;
            isLooting = false;
            weaponScript = gameObject.GetComponent<WeaponScript>();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Drop(Vector3 position)
        {
            Show();
            GetComponent<Transform>().position = position;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }


        //--------------------------------------------------------------------------------------------
        // - GETTERS / SETTERS
        //--------------------------------------------------------------------------------------------
        public string GetWeaponName()
        {
            return weaponScript ? weaponScript.GetName() : null;
        }

        public WeaponScript GetWeapon()
        {
            return weaponScript;
        }

        public Sprite GetSprite()
        {
            return itemSprite;
        }

        public void SetLootTrackerIndex(int id)
        {
            lootTrackerIndex = id;
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