using UnityEngine;

namespace BattleRobo
{
	public class WeaponScript : MonoBehaviour
	{
		[SerializeField]
		private Gun currentGun;

		[SerializeField]
		private Animator playerAnimator;

		public float currentAmmo;
		private float nextTimeToFire;

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
		}

		public bool CanFire()
		{
			if (Time.time >= nextTimeToFire)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Fire the gun and deals the gun damage
		/// </summary>
		public void Fire(Transform camTransform, int playerID)
		{
			Debug.Log("Fire " + currentGun.gunName);
			//set the next time to fire and decrease ammo
			nextTimeToFire = Time.time + currentGun.fireRate;
			currentAmmo--;

			const int layerMask = 1 << 8;
			RaycastHit shot;

			if (Physics.Raycast(camTransform.position, camTransform.forward, out shot, currentGun.range, layerMask))
			{
				Debug.Log("Hit" + shot.transform.gameObject.name);
				shot.transform.gameObject.GetComponent<PlayerScript>().TakeDamage(currentGun.damage, playerID);
			}
		}
	}
}