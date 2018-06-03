using UnityEngine;

namespace BattleRobo
{
    public class WeaponScript : MonoBehaviour
    {
        [SerializeField]
        private Gun currentGun;

        [SerializeField]
        private Animator playerAnimator;

        public PhotonView playerPhotonView;

        public float currentAmmo;
        private float nextTimeToFire;

        private void Start()
        {
            currentAmmo = currentGun.magazineSize;
        }

        private void Update()
        {
            if (!playerPhotonView) //player photon view is null
                return;

            if (!playerPhotonView.isMine)
                return;

            playerAnimator.SetLayerWeight(2, 1);
        }

        public bool CanFire()
        {
            return Time.time >= nextTimeToFire;
        }

        /// <summary>
        /// Fire the gun and deals the gun damage
        /// </summary>
        public void Fire(Transform camTransform, int playerID)
        {
            //set the next time to fire and decrease ammo
            nextTimeToFire = Time.time + currentGun.fireRate;

            if (currentAmmo > 0)
            {
                currentAmmo--;

                const int layerMask = 1 << 8;
                RaycastHit shot;

                if (Physics.Raycast(camTransform.position, camTransform.forward, out shot, currentGun.range, layerMask))
                {
                    Debug.Log("Hit" + shot.transform.gameObject.name);
                    //shot.transform.gameObject.GetComponent<PlayerScript>().TakeDamage(currentGun.damage, playerID);
                    shot.transform.gameObject.GetComponent<RoboControllerScript>().TakeDamage(currentGun.damage, playerID);
                }
            }
        }

        public string GetName()
        {
            return currentGun.name;
        }

        public int GetMagazineSize()
        {
            return currentGun.magazineSize;
        }

        public void SetCurrentAmmo(float ammoNumber)
        {
            currentAmmo = ammoNumber;
        }

        public float GetCurrentAmmo()
        {
            return currentAmmo;
        }
    }
}