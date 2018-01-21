using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : Photon.PunBehaviour
{

	[SerializeField] 
	private LevelGeneratorScript level;
	// Use this for initialization
	
	void Start ()
	{
		Vector3[] spawns = level.getSpawningPoints();
		
		GameObject player = PhotonNetwork.Instantiate("RobotWheelNetwork", spawns[Random.Range(0, spawns.Length)], Quaternion.identity, 0);

		if (player.GetPhotonView().isMine)
		{
			Debug.Log("PhotonView is Mine !");
			player.transform.Find("robot").Find("Armature").Find("torso").Find("chest").Find("Camera").GetComponent<Camera>()
				.enabled = true;
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
