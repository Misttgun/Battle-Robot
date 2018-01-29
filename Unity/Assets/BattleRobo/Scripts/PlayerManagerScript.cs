using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerScript : Photon.PunBehaviour
{

	public float Health = 100f;
	// Use this for initialization
	void OnTriggerEnter (Collider other)
	{
		if (!photonView.isMine)
			return;

		if (!other.name.Contains("bullet"))
			return;

		Health -= 1f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
