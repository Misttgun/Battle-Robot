using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : Photon.PunBehaviour
{

	[SerializeField] 
	private LevelGeneratorScript level;
	
	private void Start ()
	{
		Vector3[] spawns = level.getSpawningPoints();
		
		PhotonNetwork.Instantiate("RobotWheelNetwork", spawns[Random.Range(0, spawns.Length)], Quaternion.identity, 0);

	}
}
