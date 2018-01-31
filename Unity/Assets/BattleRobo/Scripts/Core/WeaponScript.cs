using UnityEngine;

namespace BattleRobo.Core
{
    public class WeaponScript : MonoBehaviour
    {
        /// <summary>
        /// Gun class to define the logic of a gun
        /// </summary>
        public class Gun
        {
            private readonly ushort magazineSize;
            private readonly ushort damage;
            private readonly float range;
            private readonly string name;

            public Gun(ushort magazineSize, ushort damage, float range, string name)
            {
                this.magazineSize = magazineSize;
                this.damage = damage;
                this.range = range;
                this.name = name;
            }

            public ushort MagazineSize
            {
                get { return magazineSize; }
            }

            public ushort Damage
            {
                get { return damage; }
            }

            public float Range
            {
                get { return range; }
            }

            public string Name
            {
                get { return name; }
            }
        }

        [SerializeField] private Camera camFPS;
        [SerializeField] private PhotonView playerPhotonView;

        private readonly Gun pistol = new Gun(15, 15, 125f, "Beretta");

        private void Update()
        {
            if (!playerPhotonView.isMine) return;

            if (Input.GetButtonDown("Fire1"))
            {
                Fire(pistol);
            }
        }

        /// <summary>
        /// Fire the gun and deals the gun damage
        /// </summary>
        /// <param name="currentGun">the gun selected by the user</param>
        private void Fire(Gun currentGun)
        {
            int layerMask = 1 << 8;
            layerMask = ~layerMask;
            RaycastHit shot;

            if (!Physics.Raycast(camFPS.transform.position, camFPS.transform.forward, out shot, currentGun.Range,
                layerMask))
            {
                shot.transform.gameObject.GetPhotonView().RPC("TakeDamage", PhotonTargets.All, currentGun.Damage);
            }
        }
    }
}