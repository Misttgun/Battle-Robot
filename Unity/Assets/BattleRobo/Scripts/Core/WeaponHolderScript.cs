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
        private PhotonView playerPhotonView;

        [SerializeField]
        private GameObject bullet;

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
                    if (inventoryWeapon && weapon.GetId() == inventoryWeapon.GetId())
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

        [PunRPC]
        private void BulletSpawnRPC(Vector3 destination, Vector3 spawnPos, Quaternion rotation)
        {
            var spawnedBullet = PoolManagerScript.Spawn(bullet, spawnPos, rotation);
            
            var bulletScript = spawnedBullet.GetComponent<BulletScript>();
            bulletScript.target = destination;
            bulletScript.spawnPoint = spawnPos;
            
            PoolManagerScript.Despawn(spawnedBullet, 0.5f);
        }
    }
}