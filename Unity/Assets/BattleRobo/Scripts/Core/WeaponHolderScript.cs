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

        [SerializeField]
        private RoboControllerScript _playerControllerScript;

        [SerializeField]
        private PhotonView playerPhotonView;

        private void Start()
        {
            //inventory = _playerControllerScript.GetInventory();
            //inventory.SetWeaponHolder(this);


            foreach (var weapon in equipWeapons)
            {
                weapon.playerPhotonView = playerPhotonView;
            }
        }

        public void SetWeapon(WeaponScript inventoryWeapon, float currentAmmo)
        {
            var index = 0;

            // - unequip weapon
            if (!inventoryWeapon)
            {
                playerPhotonView.RPC("EquipWeaponRPC", PhotonTargets.All, -1, 0f);
            }

            // - equip the right weapon
            else
            {
                foreach (WeaponScript weapon in equipWeapons)
                {
                    if (inventoryWeapon && weapon.GetName() == inventoryWeapon.GetName())
                    {
                        playerPhotonView.RPC("EquipWeaponRPC", PhotonTargets.All, index, currentAmmo);
                    }

                    index++;
                }
            }
        }

        public void EquipWeapon(int weaponIndex, float currentAmmo)
        {
            int index = 0;

            foreach (WeaponScript weapon in equipWeapons)
            {
                if (index == weaponIndex)
                {
                    weapon.gameObject.SetActive(true);
                    currentWeapon = weapon;
                    weapon.SetCurrentAmmo(currentAmmo);
                }

                else
                {
                    weapon.gameObject.SetActive(false);
                }

                index++;
            }

            // - unequip weapon
            if (weaponIndex == -1)
            {
                currentWeapon = null;
            }
        }

        public WeaponScript GetCurrentWeapon()
        {
            return currentWeapon;
        }
    }
}