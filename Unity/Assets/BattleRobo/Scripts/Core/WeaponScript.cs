using UnityEngine;

namespace BattleRobo.Core
{
    public class WeaponScript : MonoBehaviour
    {
        [SerializeField]
        private Gun currentGun;

        [SerializeField]
        private Camera camFPS;
        
        [SerializeField]
        private Animator playerAnimator;

        [SerializeField]
        private PhotonView playerPhotonView;

        private float nextTimeToFire;

        public float currentAmmo;

        private void Start()
        {
            currentAmmo = currentGun.magazineSize;
        }

        private void Update()
        {
            //if (!playerPhotonView.isMine) return;

            if (currentGun.twoHanded)
            {
                playerAnimator.SetLayerWeight(2, 1);
                playerAnimator.SetLayerWeight(3, 0);
            }
            else
            {
                playerAnimator.SetLayerWeight(3, 1);
                playerAnimator.SetLayerWeight(2, 0);
            }

            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                Debug.Log("Fire " + currentGun.gunName);
                nextTimeToFire = Time.time + currentGun.fireRate;
                currentAmmo--;
                Fire();
            }
        }

        /// <summary>
        /// Fire the gun and deals the gun damage
        /// </summary>
        private void Fire()
        {
            const int layerMask = 1 << 8;
            RaycastHit shot;

            if (Physics.Raycast(camFPS.transform.position, camFPS.transform.forward, out shot, currentGun.range, layerMask))
            {
                Debug.Log("Hit" + shot.transform.gameObject.name);
                //shot.transform.gameObject.GetPhotonView().RPC("TakeDamage", PhotonTargets.All, currentGun.damage);
            }
        }
    }
}