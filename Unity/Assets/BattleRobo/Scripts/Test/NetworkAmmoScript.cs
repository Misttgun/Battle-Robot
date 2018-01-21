using UnityEngine;
using UnityEngine.UI;

public class NetworkAmmoScript : Photon.PunBehaviour {

	public NetworkGunScript gun;

	[SerializeField]
	private GameObject displayAmmo;
	
	public static int currentAmmo;

	private void Start()
	{
		currentAmmo = gun.AmmoCapacity;
	}
	void Update ()
	{
		displayAmmo.GetComponent<Text>().text = "" + currentAmmo;
		if (Input.GetButtonDown("Fire1"))
		{
			currentAmmo -= 1;
		}

		if(currentAmmo==0)
		{
			Debug.Log("RELOAD !!!!"); //animation

			currentAmmo = gun.AmmoCapacity;
		}
	}
}
