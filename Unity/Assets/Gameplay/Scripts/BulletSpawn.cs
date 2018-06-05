using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawn : MonoBehaviour
{

	[SerializeField] private GameObject mg; // cube that throw bullet 
	
	[SerializeField]
	private GameObject bulletSpawn; 

	[SerializeField]
	private GameObject bullet;

	[SerializeField]
	private float bulletSpeed;


	private void Update()
	{
		if(Input.GetButtonDown("Fire2"))
		{
			bulletPropagation();
		}
	}

	private void bulletPropagation()
	{
		GameObject spawnBullet;
		spawnBullet = Instantiate(bullet, bulletSpawn.transform.position, mg.transform.rotation);

		Rigidbody bulletRgb;
		bulletRgb = spawnBullet.GetComponent<Rigidbody>();

		bulletRgb.AddForce(mg.transform.forward * -bulletSpeed);

		Destroy(spawnBullet, 3.0f);
	}
}
