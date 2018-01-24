using System.ComponentModel.Design;
using UnityEngine;

public class NetworkFireScript : Photon.PunBehaviour {

	[SerializeField]
	private Camera camFPS;

	[SerializeField]
	private LayerMask mask;

	[SerializeField]
	private GameObject bulletSpawn; 

	[SerializeField]
	private GameObject bullet;

	[SerializeField]
	private float bulletSpeed;

	public GunScript gun;

	private void Update()
	{
		if (!photonView.isMine) return;
		
		if(Input.GetButtonDown("Fire1"))
		{
			bulletPropagation();
			fire();
		}
	}
	private void fire()
	{	
		RaycastHit shot;
		if(Physics.Raycast(camFPS.transform.position, camFPS.transform.forward , out shot, gun.range, mask) && shot.rigidbody)
		{
			Debug.LogError(" hit " + shot.collider.name);
			NetworkRoboControllerScript controller = shot.collider.gameObject.GetComponentInParent<NetworkRoboControllerScript>();
			
			Debug.Log(shot.collider.gameObject.name);

			if (controller)
			{
				shot.collider.gameObject.GetComponentInParent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, 20f);
				//controller.TakeDamage(20f);
			}
			shot.rigidbody.AddForce(camFPS.transform.forward * bulletSpeed/100);
		}

	}
	private void bulletPropagation()
	{
		GameObject spawnBullet;
		spawnBullet = Instantiate(bullet, bulletSpawn.transform.position, camFPS.transform.rotation);

		Rigidbody bulletRgb;
		bulletRgb = spawnBullet.GetComponent<Rigidbody>();

		bulletRgb.AddForce(camFPS.transform.forward * bulletSpeed);

		Destroy(spawnBullet, 3.0f);
	}
}
