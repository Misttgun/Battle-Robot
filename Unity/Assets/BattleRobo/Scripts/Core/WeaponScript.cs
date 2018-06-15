using UnityEngine;

namespace BattleRobo
{
    public class WeaponScript : MonoBehaviour
    {
        [SerializeField]
        private Gun currentGun;

        [SerializeField]
        private Animator playerAnimator;

        public AudioClip weaponSound;

        [SerializeField]
        private PhotonView playerPhotonView;

        [SerializeField]
        private PhotonView weaponHolderPhotonView;

        public float currentAmmo;

        private float nextTimeToFire;

        #region Bullet Region

        [SerializeField]
        private Transform bulletSpawn;

        private Vector3 hitPoint;

        #endregion

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

            if (!(currentAmmo > 0))
                return;

            currentAmmo--;

            if (!PhotonNetwork.isMasterClient)
                return;

            const int layerMask = 1 << 8;
            RaycastHit shot;

            if (Physics.Raycast(camTransform.position, camTransform.forward, out shot, currentGun.range, layerMask))
            {
                Debug.Log("Hit" + shot.transform.gameObject.name);

                var hitRoboController = shot.transform.gameObject.GetComponent<RoboControllerScript>();
                playerPhotonView.RPC("HitMarkerRPC", PhotonTargets.AllViaServer);
                hitRoboController.TakeDamage(currentGun.damage, playerID);
                hitRoboController.ShowDamageIndicator(camTransform.position);
                hitPoint = shot.point;
            }
            else
            {
                hitPoint = camTransform.position + camTransform.forward * currentGun.range;
            }

            weaponHolderPhotonView.RPC("BulletSpawnRPC", PhotonTargets.All, hitPoint, bulletSpawn.position, camTransform.rotation);
        }

        public string GetName()
        {
            return currentGun.gunName;
        }

        public int GetId()
        {
            return currentGun.gunId;
        }

        public int GetMagazineSize()
        {
            return currentGun.magazineSize;
        }

        public void SetCurrentAmmo(float ammoNumber)
        {
            currentAmmo = ammoNumber;
        }
    }
}