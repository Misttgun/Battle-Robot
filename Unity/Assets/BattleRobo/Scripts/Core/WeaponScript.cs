using System.Configuration;
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

        public float currentAmmo;

        private float nextTimeToFire;

        private const int layerMask = 1 << 8;

        private void Awake()
        {
            currentAmmo = currentGun.magazineSize;
        }

        private void Update()
        {
            if (!playerPhotonView) //player photon view is null
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

            RaycastHit shot;

            if (Physics.Raycast(camTransform.position, camTransform.forward, out shot, currentGun.range, layerMask))
            {
                Debug.Log("Hit" + shot.transform.gameObject.name);

                var hitRoboBodyPart = shot.transform.gameObject.GetComponent<HealthScript>();
                playerPhotonView.RPC("HitMarkerRPC", PhotonTargets.AllViaServer);
                hitRoboBodyPart.TakeDamage(currentGun.damage, playerID);
                hitRoboBodyPart.ShowDamageIndicator(camTransform.position);
            }
        }

        public int GetId()
        {
            return currentGun.gunId;
        }

        public void SetCurrentAmmo(float ammoNumber)
        {
            currentAmmo = ammoNumber;
        }
    }
}