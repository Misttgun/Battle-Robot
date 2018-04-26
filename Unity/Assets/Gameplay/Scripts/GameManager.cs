using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private void Start()
	{
		PhotonNetwork.Instantiate("Robo_Test", new Vector3(0, 10, 7), Quaternion.identity, 0);
	}
}
