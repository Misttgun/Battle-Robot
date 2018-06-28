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

        public void SetWeapon(WeaponScript inventoryWeapon, float currentAmmo)
        {
            //unequip weapon
            if (!inventoryWeapon)
            {
                playerPhotonView.RPC("EquipWeaponRPC", PhotonTargets.All, -1, 0f);
            }
            else
            {
                for (int i = 0; i < equipWeapons.Length; i++)
                {
                    if (!inventoryWeapon || equipWeapons[i].GetId() != inventoryWeapon.GetId()) 
                        continue;
                    
                    playerPhotonView.RPC("EquipWeaponRPC", PhotonTargets.All, i, currentAmmo);
                    break;
                }
            }
        }

        public void EquipWeapon(int weaponIndex, float currentAmmo)
        {
            for (int i = 0; i < equipWeapons.Length; i++)
            {
                if (i == weaponIndex)
                {
                    equipWeapons[i].gameObject.SetActive(true);
                    currentWeapon = equipWeapons[i];
                    equipWeapons[i].SetCurrentAmmo(currentAmmo);
                }
                else
                {
                    equipWeapons[i].gameObject.SetActive(false);
                }
            }

            //unequip weapon
            if (weaponIndex == -1)
            {
                currentWeapon = null;
            }
        }
    }
}