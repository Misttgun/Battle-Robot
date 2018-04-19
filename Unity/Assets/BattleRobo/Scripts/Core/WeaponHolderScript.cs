using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BattleRobo
{
    public class WeaponHolderScript : MonoBehaviour
    {
        //TODO Rendre le code plus propre avec le system d'inventaire et de pickup
        public WeaponScript[] equipWeapons;


        public WeaponScript currentWeapon;

        private PlayerInventory inventory;

        [SerializeField]
        private PhotonView playerPhotonView;

        private void Start()
        {
            if (!playerPhotonView.isMine)
                return;
            
            inventory = playerPhotonView.gameObject.GetComponent<PlayerScript>().GetInventory();
            inventory.SetWeaponHolder(this);
            
            
            foreach (var weapon in equipWeapons)
            {
                weapon.playerPhotonView = playerPhotonView;
            }
        }
        
        public void SetWeapon(WeaponScript inventoryWeapon)
        {
            var index = 0;

            // - unequip weapon
            if (!inventoryWeapon)
            {
                playerPhotonView.RPC("EquipWeaponRPC", PhotonTargets.All, -1);
            }

            // - equip the right weapon
            else
            {
                foreach (WeaponScript weapon in equipWeapons)
                {
                    if (inventoryWeapon && weapon.GetName() == inventoryWeapon.GetName())
                    {
                        playerPhotonView.RPC("EquipWeaponRPC", PhotonTargets.All, index);
                    }

                    index++;
                }   
            }
        }

        public void EquipWeapon(int weaponIndex)
        {
            int index = 0;
            
            foreach (WeaponScript weapon in equipWeapons)
            {
                if (index == weaponIndex)
                {
                    weapon.gameObject.SetActive(true);
                    currentWeapon = weapon;
                }

                else
                {
                    weapon.gameObject.SetActive(false);
                }

                index++;
            }
        }
    }
}