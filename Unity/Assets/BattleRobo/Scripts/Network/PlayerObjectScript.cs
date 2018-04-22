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
        [SerializeField] private int id;
        [SerializeField] private int maxStackSize;
        [SerializeField] private Sprite itemSprite;

        private Transform position;
        private PhotonView myPhotonView;
        private WeaponScript weaponScript;
        private int lootTrackerIndex;

        public void Start()
        {
            weaponScript = gameObject.GetComponent<WeaponScript>();
        }

        public void Take()
        {
            // - find how to hide the gun for all player
            Hide();

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

        public int GetMaxStack()
        {
            return maxStackSize;
        }
    }
}